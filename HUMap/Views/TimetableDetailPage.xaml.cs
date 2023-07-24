namespace HUMap.Views;

public sealed partial class TimetableDetailPage
{
    public TimetableDetailPage(TimetableDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnViewLocationClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        if (button.BindingContext is not TimetableItem item) return;
        var location = item.Location;
        Preferences.Default.Set("location", location);
        await Shell.Current.GoToAsync("///MapPage");
    }
}