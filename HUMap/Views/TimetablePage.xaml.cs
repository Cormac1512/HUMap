namespace HUMap.Views;

public partial class TimetablePage : ContentPage
{
    private TimetableViewModel ViewModel;

    public TimetablePage(TimetableViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel = viewModel;
        ViewModel.LoadDataAsync();
    }
}