namespace HUMap.Views;

public sealed partial class AboutPage
{
    private AboutViewModel _vm;

    public AboutPage(AboutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _vm = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var versionLabel = this.FindByName<Label>("version");
        if (versionLabel != null)
            versionLabel.Text = $"v{VersionTracking.CurrentVersion}";
    }
}