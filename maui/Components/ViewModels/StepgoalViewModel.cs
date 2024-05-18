using System.ComponentModel;
using System.Net.Http.Json;
using maui.Components.Models;
using System.Linq;
/*using ABI.Windows.Devices.Sensors;*/

public class StepgoalViewModel : INotifyPropertyChanged
{
    //default values
    private readonly HttpClient _httpClient;
    private List<StepgoalModel> _allStepgoals;
    private int _goal;
    private DateTime _date = DateTime.Now;
    private int _progress = 0;
    private bool _achieved = false;
    public bool IsSuccessful { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public StepgoalViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _progress = 0; // Initialize progress to 0 by default
    }
    
    public void StartCounting()
    {
        Plugin.Maui.Pedometer.Pedometer.ReadingChanged += (sender, reading) =>
        {
            Console.WriteLine(reading.NumberOfSteps);
        };

        Plugin.Maui.Pedometer.Pedometer.Default.Start();
    }

    public int Goal
    {
        get => _goal;
        set
        {
            _goal = value;
            OnPropertyChanged(nameof(Goal));
        }
    }

    public DateTime Date
    {
        get => _date;
    }

    public int Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            OnPropertyChanged(nameof(Progress));
        }
    }

    public bool Achieved
    {
        get => _achieved;
        set
        {
            _achieved = value;
            OnPropertyChanged(nameof(Achieved));
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
            var response = await _httpClient.PostAsync("https://7mqh6pqj-5041.euw.devtunnels.ms/api/stepgoals", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Stepgoal created successfully!");
                Goal = 0; // Reset form
                IsSuccessful = true;  // Set flag for notification
            }
            else
            {
                Console.WriteLine($"Error creating stepgoal: {response.StatusCode}");
                IsSuccessful = false; // Set flag for error notification
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            IsSuccessful = false; // Set flag for error notification
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    //delete stepgoals
    public async Task DeleteTodaysGoals()
    {
        // 1. Get all stepgoals for today
        var today = DateTime.Today;
        var url = "http://localhost:5041/api/stepgoals"; // Assuming all stepgoals are retrieved here

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error retrieving stepgoals: {response.StatusCode}");
                return; // Handle error appropriately (e.g., display message to user)
            }

            var allStepgoals = await response.Content.ReadFromJsonAsync<List<StepgoalModel>>();

            // 2. Filter for goals with today's date
            var todaysGoals = allStepgoals.Where(sg => sg.Date.Date == today.Date).ToList();

            // 3. Delete each goal individually
            int deletedGoalsCount = todaysGoals.Count;
            bool isDeletionSuccessful = deletedGoalsCount > 0;

            foreach (var goal in todaysGoals)
            {
                var deleteUrl = $"http://localhost:5041/api/stepgoals/{goal.Id}";
                var deleteResponse = await _httpClient.DeleteAsync(deleteUrl);

                if (!deleteResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error deleting stepgoal {goal.Id}: {deleteResponse.StatusCode}");
                    // Handle individual deletion errors (optional)
                }
            }

            // 4. Handle success or no goals found
            if (isDeletionSuccessful)
            {
                Console.WriteLine($"Successfully deleted {deletedGoalsCount} stepgoals for today.");
                // Update UI with success message (see integration steps)
            }
            else
            {
                Console.WriteLine("No stepgoals found for today to delete.");
                // Consider displaying a message indicating no goals were found
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Handle general errors (e.g., network issues)
        }
    }
    
    //update works not yet
    public async Task UpdateStepgoal()
    {
        if (GoalToUpdate != null)
        {
            var updateUrl = $"http://localhost:5041/api/stepgoals/{GoalToUpdate.Id}";

            try
            {
                var updatedGoal = new StepgoalModel { Goal = Goal };
                var content = JsonContent.Create(updatedGoal);
                var response = await _httpClient.PutAsync(updateUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Stepgoal updated successfully!");
                    IsSuccessful = true;  // Set flag for notification
                }
                else
                {
                    Console.WriteLine($"Error updating stepgoal: {response.StatusCode}");
                    IsSuccessful = false; // Set flag for error notification
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                IsSuccessful = false; // Set flag for error notification
            }
        }
        else
        {
            // Handle case where no goal found for today (optional)
            Console.WriteLine("No stepgoal found for today.");
        }
    }



    
    
    
    //displays today's goal
    public List<StepgoalModel> AllStepgoals // Expose all goals (optional)
    {
        get => _allStepgoals;
        set
        {
            _allStepgoals = value;
            OnPropertyChanged(nameof(AllStepgoals));
        }
    }
    public StepgoalModel GoalToUpdate { get; private set; }

    public async Task LoadTodayStepgoals() // New method for today's goals
    {
        try
        {
            var allGoals = await _httpClient.GetFromJsonAsync<List<StepgoalModel>>("http://localhost:5041/api/stepgoals"); // Assuming all stepgoals are retrieved here
            if (allGoals != null)
            {
                AllStepgoals = allGoals;
                // Filter for today's goals
                var todaysGoals = allGoals.Where(sg => sg.Date.Date == DateTime.Today.Date).ToList();
                IsSuccessful = todaysGoals.Count <= 1; // Check for max 1 goal
                ErrorMessage = todaysGoals.Count > 1 ? "You have set more than 1 step goal for today. Please keep it to 1 or less." : "";
                GoalToUpdate = todaysGoals.FirstOrDefault();
            }
            else
            {
                IsSuccessful = false; // Set flag for error
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            IsSuccessful = false; // Set flag for error
        }
    }

    /*public bool IsSuccessful { get; private set; } // Flag for successful data retrieval*/
    public string ErrorMessage { get; private set; }

}
