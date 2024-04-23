using System.ComponentModel;
using System.Net.Http.Json;
using maui.Components.Models;
using System.Linq;

public class StepgoalViewModel : INotifyPropertyChanged
{
    //default values
    private readonly HttpClient _httpClient;
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
            var response = await _httpClient.PostAsync("http://localhost:5041/api/stepgoals", content);

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
    
    public async Task UpdateStepgoal(StepgoalModel updatedGoal)
    {
        // Construct the update URL with the goal ID
        var updateUrl = $"http://localhost:5041/api/stepgoals/{updatedGoal.Id}";

        try
        {
            var content = JsonContent.Create(updatedGoal);
            var response = await _httpClient.PutAsync(updateUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Stepgoal updated successfully!");
                Goal = updatedGoal.Goal; // Update local goal value
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

}
