using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
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

        private async Task LoadBlogs()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5041/api/blogs");

                if (response.IsSuccessStatusCode)
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    var blogs = await JsonSerializer.DeserializeAsync<IEnumerable<BlogModel>>(contentStream);
                    if (blogs != null)
                    {
                        // Only fetch blog content, not titles
                        Blogs = new List<BlogModel>(blogs.Select(b => new BlogModel { Content = b.Content }));
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve blogs: {response.StatusCode}");
                    // Handle error or display a user-friendly message
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading blogs: {ex.Message}");
                // Handle error or display a user-friendly message
            }
        }

        public async Task<BlogModel?> GetBlogByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://localhost:5041/api/blogs/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    return await JsonSerializer.DeserializeAsync<BlogModel>(contentStream);
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve blog with ID {id}: {response.StatusCode}");
                    // Handle error or display a user-friendly message
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching blog with ID {id}: {ex.Message}");
                // Handle error or display a user-friendly message
                return null;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
