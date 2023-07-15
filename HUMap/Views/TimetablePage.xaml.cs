namespace HUMap.Views;

public partial class TimetablePage : ContentPage
{
    private readonly TimetableViewModel ViewModel;

    public TimetablePage(TimetableViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel = viewModel;
        viewModel.LoadDataAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadDataAsync();
    }
}