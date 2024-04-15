using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;



namespace maui.components.ViewModels
{
    public class LoginViewModel
    {
        
        public string Email { get; set; }
        public string Password { get; set; }

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
                    // You may want to consider storing authentication details securely

                    OnLoginSuccessful?.Invoke(this, jsonResponse); // Raise event for successful login
                    
                    /*
                    await Shell.Current.GoToAsync("/home");
                    */
                        
                  
                    //redirect here
                    
                    return jsonResponse; // Optional: Return response data for further processing
                }
                else
                {
                    /*
                    await Shell.Current.GoToAsync("/home");
                    */
                    return "Invalid login credentials. Please try again."; // Generic error message
                }
            }
        }
    }
}