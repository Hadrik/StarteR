using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using StarteR.Models.Steps;

namespace StarteR.Models;

public partial class FlowModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private bool _isEnabled = true;
    
    [ObservableProperty]
    private ObservableCollection<StepModelBase> _steps = [];
}