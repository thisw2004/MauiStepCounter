using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Threading.Tasks;
using maui.Components.Models;

public class StepgoalViewModel : INotifyPropertyChanged
{
    private readonly HttpClient _httpClient;

    public event PropertyChangedEventHandler PropertyChanged;

    public StepgoalViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
        StepGoals = new List<StepgoalModel>();
        LoadStepGoals();
    }

    private List<StepgoalModel> _stepgoals;
    public List<StepgoalModel> Stepgoals
    {
        get => _stepgoals;
        set
        {
            _stepgoals = value;
            OnPropertyChanged(nameof(_stepgoals));
        }
    }

    private async Task LoadStepGoals()
    {
        try
        {
            var blogs = await _httpClient.GetFromJsonAsync<IEnumerable<BlogModel>>("http://localhost:5041/api/blogs");
            if (blogs != null)
                Stepgoals = Stepgoals.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            // Handle error
        }
    }


    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}