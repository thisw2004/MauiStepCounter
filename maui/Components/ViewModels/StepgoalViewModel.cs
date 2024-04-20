using System.ComponentModel;
using System.Net.Http.Json;
using maui.Components.Models;

public class StepgoalViewModel : INotifyPropertyChanged
{
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
        set => _date = value;
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
                // Handle successful creation (e.g., clear form, display success message)
                Console.WriteLine("Stepgoal created successfully!");
                Goal = 0; // Reset form after successful creation
            }
            else
            {
                Console.WriteLine($"Error creating stepgoal: {response.StatusCode}");
                // Handle errors (e.g., display error message to user)
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Handle errors (e.g., display error message to user)
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
