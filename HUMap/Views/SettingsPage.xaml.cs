namespace HUMap.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _vm;

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _vm = viewModel;
        if (Preferences.Default.ContainsKey("ICalUrl")) entry.Text = Preferences.Default.Get("ICalUrl", "");
    }

    /// <summary>
    ///     This method sets the iCal URL based on value stored in the entry text field.
    ///     If the URL is valid, it is stored in _vm.ICalUrl and saved to preferences.
    ///     If the URL is not valid, an error alert is displayed to the user.
    /// </summary>
    /// <param name="sender">Event sender object</param>
    /// <param name="args">Event arguments</param>
    private async void SetLink(object sender, EventArgs args)
    {
        if (entry.Text == null) return;

        if (Uri.IsWellFormedUriString(entry.Text, UriKind.Absolute))
        {
            _vm.ICalUrl = entry.Text;
            Preferences.Default.Set("ICalUrl", _vm.ICalUrl);
            await DisplayAlert("Successful", "New URL set", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Invalid URL", "OK");
        }
    }
}