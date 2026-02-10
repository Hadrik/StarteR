using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StarteR.Models;

public partial class AppModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<FlowModel> _flows = [];
}