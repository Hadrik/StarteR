using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarteR.Models;
using StarteR.Services;

namespace StarteR.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly FlowRunnerService _flowRunner;
    
    [ObservableProperty]
    private ObservableCollection<FlowModel> _flows;
    
    [ObservableProperty]
    private FlowModel? _currentFlow;

    [ObservableProperty]
    private FlowEditorViewModel? _currentEditor;

    public MainWindowViewModel(AppModel appModel, FlowRunnerService flowRunner)
    {
        _flowRunner = flowRunner;
        Flows = new ObservableCollection<FlowModel>(appModel.Flows);
    }

    partial void OnCurrentFlowChanged(FlowModel? value)
    {
        CurrentEditor = value != null ? new FlowEditorViewModel(value, _flowRunner, RemoveFlow) : null;
    }

    [RelayCommand]
    private void AddFlow()
    {
        var flow = new FlowModel
        {
            Name = "New Flow",
        };
        Flows.Add(flow);
        CurrentFlow = Flows.Last();
    }
    
    private void RemoveFlow(FlowModel flow)
    {
        Flows.Remove(flow);
        CurrentFlow = Flows.FirstOrDefault();
    }
}