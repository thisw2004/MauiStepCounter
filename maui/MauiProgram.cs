/*using Windows.Devices.Sensors;*/
using maui.components.ViewModels;
using Microsoft.Extensions.Logging;

namespace maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();
        // Inside ConfigureServices method in Program.cs or Startup.cs
        builder.Services.AddScoped<HttpClient>();

        builder.Services.AddScoped<BlogViewModel>();
        builder.Services.AddScoped<StepgoalViewModel>();
        

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}