namespace HUMap.Views;

public partial class TimetableDetailPage : ContentPage
{
	public TimetableDetailPage(TimetableDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
