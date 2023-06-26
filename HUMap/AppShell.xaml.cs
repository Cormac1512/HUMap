namespace HUMap;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(TimetableDetailPage), typeof(TimetableDetailPage));
	}
}
