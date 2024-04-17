using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace maui.components.ViewModels
{
    public class BlogViewModel
    {
        private readonly NavigationManager _navigationManager;

        public BlogViewModel(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public event EventHandler<string> OnBlogsRetrieved; // Event for retrieving blogs

        public async Task<string> ShowBlogs()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync("http://localhost:5041/api/blogs"); // Adjusted endpoint

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        OnBlogsRetrieved?.Invoke(this, jsonResponse); // Raise event for blogs retrieval
                        return null; // No error, return null
                    }
                    else
                    {
                        // Handle error response
                        return $"Failed to retrieve blogs. Error: {response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                return $"Failed to retrieve blogs. Exception: {ex.Message}";
            }
        }
    }
}