namespace HUMap.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class TimetableDetailViewModel : BaseViewModel
{
    [ObservableProperty]
    private TimetableItem item;
}