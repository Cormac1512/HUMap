namespace HUMap.ViewModels;

[QueryProperty(nameof(item), "Item")]
public partial class TimetableDetailViewModel : BaseViewModel
{
    [ObservableProperty]
    private TimetableItem item;
}