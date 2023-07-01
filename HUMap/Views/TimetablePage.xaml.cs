namespace HUMap.Views;

public partial class TimetablePage : ContentPage
{
    private TimetableViewModel ViewModel;

    public TimetablePage(TimetableViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel = viewModel;
        var settingsViewModel = new SettingsViewModel();
        if (!Preferences.Default.ContainsKey("ICalUrl"))
        {
            Navigation.PushAsync(new SettingsPage(settingsViewModel));
            DisplayAlert("Setup", "In order to use the timetable, an ICal link must be set. The map function can still be used without one", "OK");
        }
        else
        {
            ViewModel.LoadDataAsync();
        }
    }
}