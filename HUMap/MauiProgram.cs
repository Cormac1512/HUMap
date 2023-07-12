using CommunityToolkit.Maui;

namespace HUMap;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("FontAwesome6FreeBrands.otf", "FontAwesomeBrands");
                fonts.AddFont("FontAwesome6FreeRegular.otf", "FontAwesomeRegular");
                fonts.AddFont("FontAwesome6FreeSolid.otf", "FontAwesomeSolid");
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<TimetableService>();
        builder.Services.AddTransient<TimetableDetailViewModel>();
        builder.Services.AddTransient<TimetableDetailPage>();

        builder.Services.AddSingleton<TimetableViewModel>();

        builder.Services.AddSingleton<TimetablePage>();

        builder.Services.AddSingleton<MapViewModel>();

        builder.Services.AddSingleton<MapPage>();

        builder.Services.AddSingleton<AboutViewModel>();

        builder.Services.AddSingleton<AboutPage>();

        builder.Services.AddSingleton<SettingsViewModel>();

        builder.Services.AddSingleton<SettingsPage>();

        return builder.Build();
    }
}