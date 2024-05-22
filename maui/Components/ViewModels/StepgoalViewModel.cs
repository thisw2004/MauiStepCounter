﻿using System.ComponentModel;
using System.Net.Http.Json;
using maui.Components.Models;
using System.Linq;
using System.Runtime.CompilerServices;
using Plugin.LocalNotification;
using Plugin.Maui.Pedometer;
using System.Threading.Tasks;
using System;

public class StepgoalViewModel : INotifyPropertyChanged
{
    private readonly HttpClient _httpClient;
    private List<StepgoalModel> _allStepgoals;
    private int _goal;
    private DateTime _date = DateTime.Now;
    private int _progress = 0;
    private bool _achieved = false;
    private int _numberOfSteps;
    private string Url = "https://3gtfp6xc-5041.euw.devtunnels.ms";
    public bool IsSuccessful { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public StepgoalViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _progress = 0; // Initialize progress to 0 by default
    }

    public int Goal
    {
        get => _goal;
        set
        {
            if (_goal != value)
            {
                _goal = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StepsRemaining));
            }
        }
    }

    public DateTime Date => _date;

    public int Progress
    {
        get => _progress;
        set
        {
            if (_progress != value)
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
    }

    public bool Achieved
    {
        get => _achieved;
        set
        {
            if (_achieved != value)
            {
                _achieved = value;
                OnPropertyChanged();
            }
        }
    }

    public int NumberOfSteps
    {
        get => _numberOfSteps;
        set
        {
            if (_numberOfSteps != value)
            {
                _numberOfSteps = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StepsRemaining));
                CheckMilestones(); // Check and send notifications when steps are updated
            }
        }
    }

    public int StepsRemaining
    {
        get
        {
            int remaining = Goal - NumberOfSteps;
            return remaining < 0 ? 0 : remaining;
        }
    }

    public List<StepgoalModel> AllStepgoals
    {
        get => _allStepgoals;
        set
        {
            if (_allStepgoals != value)
            {
                _allStepgoals = value;
                OnPropertyChanged();
            }
        }
    }

    public StepgoalModel GoalToUpdate { get; private set; }

    public string ErrorMessage { get; private set; }

    public async Task LoadTodayStepgoals()
    {
        try
        {
            var allGoals = await _httpClient.GetFromJsonAsync<List<StepgoalModel>>(Url + "/api/stepgoals");
            if (allGoals != null)
            {
                AllStepgoals = allGoals;
                var todaysGoals = allGoals.Where(sg => sg.Date.Date == DateTime.Today.Date).ToList();
                IsSuccessful = todaysGoals.Count <= 1;
                ErrorMessage = todaysGoals.Count > 1 ? "You have set more than 1 step goal for today. Please keep it to 1 or less." : "";
                GoalToUpdate = todaysGoals.FirstOrDefault();
                Goal = GoalToUpdate?.Goal ?? 0;
            }
            else
            {
                IsSuccessful = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching step goals: {ex.Message}");
            IsSuccessful = false;
        }
    }

    public async Task CreateStepgoal()
    {
        var stepgoal = new StepgoalModel
        {
            Goal = Goal,
            Date = Date,
            Progress = Progress,
            Achieved = Achieved
        };

        try
        {
            var content = JsonContent.Create(stepgoal);
            var response = await _httpClient.PostAsync(Url + "/api/stepgoals", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Stepgoal created successfully!");
                Goal = 0;
                IsSuccessful = true;
            }
            else
            {
                Console.WriteLine($"Error creating stepgoal: {response.StatusCode}");
                IsSuccessful = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            IsSuccessful = false;
        }
    }

    public async Task DeleteTodaysGoals()
    {
        var today = DateTime.Today;
        var url = Url + "/api/stepgoals";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error retrieving stepgoals: {response.StatusCode}");
                return;
            }

            var allStepgoals = await response.Content.ReadFromJsonAsync<List<StepgoalModel>>();
            var todaysGoals = allStepgoals.Where(sg => sg.Date.Date == today.Date).ToList();

            int deletedGoalsCount = todaysGoals.Count;
            bool isDeletionSuccessful = deletedGoalsCount > 0;

            foreach (var goal in todaysGoals)
            {
                var deleteUrl = $"{Url}/api/stepgoals/{goal.Id}";

                var deleteResponse = await _httpClient.DeleteAsync(deleteUrl);

                if (!deleteResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error deleting stepgoal {goal.Id}: {deleteResponse.StatusCode}");
                }
            }

            if (isDeletionSuccessful)
            {
                Console.WriteLine($"Successfully deleted {deletedGoalsCount} stepgoals for today.");
            }
            else
            {
                Console.WriteLine("No stepgoals found for today to delete.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public async Task UpdateStepgoal()
    {
        if (GoalToUpdate != null)
        {
            var updateUrl = $"{Url}/api/stepgoals/{GoalToUpdate.Id}";

            try
            {
                var updatedGoal = new StepgoalModel { Goal = Goal };
                var content = JsonContent.Create(updatedGoal);
                var response = await _httpClient.PutAsync(updateUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Stepgoal updated successfully!");
                    IsSuccessful = true;
                }
                else
                {
                    Console.WriteLine($"Error updating stepgoal: {response.StatusCode}");
                    IsSuccessful = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                IsSuccessful = false;
            }
        }
        else
        {
            Console.WriteLine("No stepgoal found for today.");
        }
    }
    public async Task LoadTodayStepgoalsAndStartCounting()
    {
        try
        {
            await LoadTodayStepgoals();
            StartCounting();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading step goals and starting counting: {ex.Message}");
        }
    }
    public void StartCounting()
    {
        try
        {
            Pedometer.ReadingChanged += (sender, reading) =>
            {
                // Ensure updates to UI are on the main thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    NumberOfSteps = reading.NumberOfSteps;
                    Console.WriteLine(reading.NumberOfSteps);
                });
            };

            Pedometer.Default.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting pedometer: {ex.Message}");
        }
    }

    private async void CheckMilestones()
    {
        if (GoalToUpdate == null) return;

        try
        {
            if (NumberOfSteps >= GoalToUpdate.Goal && !Achieved)
            {
                Achieved = true;
                await ShowNotification("You have reached your goal!", "Hey! Recently you've reached your goal! Wanna check out?");
            }
            else if (NumberOfSteps >= (GoalToUpdate.Goal / 4) && NumberOfSteps < (GoalToUpdate.Goal / 2))
            {
                await ShowNotification("You're on 1/4!", "Keep going! You're doing great!");
            }
            else if (NumberOfSteps >= (GoalToUpdate.Goal / 2) && NumberOfSteps < ((GoalToUpdate.Goal / 4) * 3))
            {
                await ShowNotification("You're on 1/2!", "Halfway there! Keep pushing!");
            }
            else if (NumberOfSteps >= ((GoalToUpdate.Goal / 4) * 3) && NumberOfSteps < GoalToUpdate.Goal)
            {
                await ShowNotification("You are almost there (3/4)!", "Just a little more to reach your goal!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking milestones: {ex.Message}");
        }
    }

    private async Task ShowNotification(string title, string message)
    {
        try
        {
            var request = new NotificationRequest
            {
                NotificationId = new Random().Next(1000, 9999),
                Title = title,
                Subtitle = "Progress Update",
                Description = message,
                BadgeNumber = 1,
                Schedule = null
            };

            await LocalNotificationCenter.Current.Show(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing notification: {ex.Message}");
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
