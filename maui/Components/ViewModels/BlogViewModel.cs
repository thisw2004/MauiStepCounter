using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Threading.Tasks;
using maui.Components.Models;

public class BlogViewModel : INotifyPropertyChanged
{
    private readonly HttpClient _httpClient;

    public event PropertyChangedEventHandler PropertyChanged;

    public BlogViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
        Blogs = new List<BlogModel>();
        Task.Run(async () => await LoadBlogs());
    }

    private List<BlogModel> _blogs;
    public List<BlogModel> Blogs
    {
        get => _blogs;
        set
        {
            _blogs = value;
            OnPropertyChanged(nameof(Blogs));
        }
    }

    private async Task LoadBlogs()
    {
        try
        {
            var blogs = await _httpClient.GetFromJsonAsync<IEnumerable<BlogModel>>("http://localhost:5041/api/blogs");
            if (blogs != null)
                Blogs = blogs.ToList();
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