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

        private string Url = "https://3j1vftcr-5041.euw.devtunnels.ms";
        public string? Email { get; set; }
        public string? Password { get; set; }

        public event EventHandler<string> OnLoginSuccessful; 

        public async Task<string> Login()
        
        {
            using (var httpClient = new HttpClient())
            {
                var loginDto = new { Username = Email, Password };
                var json = JsonSerializer.Serialize(loginDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //added dev tunnel for working in android and other xamarin using platforms (like ios)
                var response = await httpClient.PostAsync(Url+"/api/account/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    

                    OnLoginSuccessful?.Invoke(this, jsonResponse); 

                    _navigationManager.NavigateTo("/home");
                    

                    return null;
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