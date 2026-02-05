using System.Collections.ObjectModel;
using StarteR.Models.Steps;

namespace StarteR.Models;

public class FlowModel
{
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public ObservableCollection<StepModelBase> Steps { get; set; } = [];
}