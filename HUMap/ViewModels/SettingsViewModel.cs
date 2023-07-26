using System.Windows.Input;

namespace HUMap.ViewModels;

public sealed partial class SettingsViewModel : BaseViewModel
{
    public string ICalUrl { get; set; }
    public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
}