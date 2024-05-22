using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using maui.Components.Models;

namespace maui.components.ViewModels
{
    public class BlogViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private List<BlogModel> _blogs;
        private BlogModel? _selectedBlog; // Added SelectedBlog property

        public event PropertyChangedEventHandler PropertyChanged;
        private string Url = "https://3gtfp6xc-5041.euw.devtunnels.ms";

        public BlogViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _blogs = new List<BlogModel>();
            LoadBlogs();
        }

        public List<BlogModel> Blogs
        {
            get => _blogs;
            set
            {
                _blogs = value;
                OnPropertyChanged(nameof(Blogs));
            }
        }
        

        // SelectedBlog property
        public BlogModel? SelectedBlog
        {
            get => _selectedBlog;
            set
            {
                _selectedBlog = value;
                OnPropertyChanged(nameof(SelectedBlog));
            }
        }

        public async Task LoadBlogs()
        {
            try
            {
                var blogs = await _httpClient.GetFromJsonAsync<IEnumerable<BlogModel>>(Url+"/api/blogs");
                if (blogs != null)
                    Blogs = blogs.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Handle error
            }
        }

        public async Task<BlogModel?> GetBlogByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{Url}/api/blogs/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    // Ensure property names match the API response exactly (case-sensitive)
                    var blog = await JsonSerializer.DeserializeAsync<BlogModel>(contentStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return blog;
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve blog with ID {id}: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching blog with ID {id}: {ex.Message}");
                return null;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
