namespace HUMap.Views;

public partial class TimetablePage : ContentPage
{
	TimetableViewModel ViewModel;

	public TimetablePage(TimetableViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = ViewModel = viewModel;
	}

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		await ViewModel.LoadDataAsync();
	}
}
