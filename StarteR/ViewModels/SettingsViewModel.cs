using StarteR.Services;

namespace StarteR.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    public bool IsAutoStartEnabled
    {
        get => AutoStartService.IsInStartup();
        set
        {
            if (value)
            {
                AutoStartService.AddToStartup(["--run"]);
            }
            else
            {
                AutoStartService.RemoveFromStartup();
            }
            OnPropertyChanged();
        }
    }
}