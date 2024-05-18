using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;

namespace maui.components.ViewModels
{
    public class RegisterViewModel
    {
        
        private readonly NavigationManager _navigationManager;
        
        public RegisterViewModel( NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        
        /*??*/
        public event EventHandler<string> WhenRegisterSuccesful; // Event for successful register

        public async Task<string> Register()
        
        {
            using (var httpClient = new HttpClient())
            {
                var RegisterDto = new { Username = Username,Email = Email, Password };
                var json = JsonSerializer.Serialize(RegisterDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://sjj4fhvm-5041.euw.devtunnels.ms/api/account/register", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    // Handle successful login response (e.g., deserialize data)

                    WhenRegisterSuccesful?.Invoke(this, jsonResponse); // Raise event for successful login

                    _navigationManager.NavigateTo("/home");
                    

                    return null; // Optional: Return response data for further processing
                }
                else
                {
                    string errorMessage;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        /*TODO: msg is here??*/
                        errorMessage = "Invalid r credentials.";
                    }
                    else
                    {
                        errorMessage = $"Register failed. Error: {response.ReasonPhrase}";
                    }

                    return errorMessage;
                }
            }
        }
    }
}
