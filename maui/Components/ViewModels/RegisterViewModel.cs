﻿using System;
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

        private string Url = "https://3j1vftcr-5041.euw.devtunnels.ms";

        /*??*/
        public event EventHandler<string> WhenRegisterSuccesful; 

        public async Task<string> Register()
        
        {
            using (var httpClient = new HttpClient())
            {
                var RegisterDto = new { Username = Username,Email = Email, Password };
                var json = JsonSerializer.Serialize(RegisterDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(Url+"/api/account/register", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
               

                    WhenRegisterSuccesful?.Invoke(this, jsonResponse); 

                    _navigationManager.NavigateTo("/home");
                    

                    return null; 
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
