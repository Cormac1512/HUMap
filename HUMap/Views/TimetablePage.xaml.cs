namespace HUMap.Views;

public sealed partial class TimetablePage
{
    private readonly TimetableViewModel _viewModel;

    public TimetablePage(TimetableViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        viewModel.LoadDataAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadDataAsync();
    }
}