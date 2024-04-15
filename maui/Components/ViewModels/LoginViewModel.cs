using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Authentication; 


namespace maui.components.ViewModels
{
    public class LoginViewModel
    {
        private readonly NavigationManager _navigationManager;

        public LoginViewModel(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public string? Email { get; set; }
        public string? Password { get; set; }

        public event EventHandler<string> OnLoginSuccessful; // Event for successful login

        public async Task<string> Login()
        
        {
            using (var httpClient = new HttpClient())
            {
                var loginDto = new { Username = Email, Password };
                var json = JsonSerializer.Serialize(loginDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://localhost:5041/api/account/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    // Handle successful login response (e.g., deserialize data)

                    OnLoginSuccessful?.Invoke(this, jsonResponse); // Raise event for successful login

                    _navigationManager.NavigateTo("/home");
                    

                    return null; // Optional: Return response data for further processing
                }
                else
                {
                    string errorMessage;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        errorMessage = "Invalid login credentials.";
                    }
                    else
                    {
                        errorMessage = $"Login failed. Error: {response.ReasonPhrase}";
                    }

                    return errorMessage;
                }
            }
        }
    }

}