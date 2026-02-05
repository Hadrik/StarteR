using System.Collections.ObjectModel;
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
        CurrentEditor = value != null ? new FlowEditorViewModel(value, _flowRunner) : null;
    }
    
    [RelayCommand]
    private void SelectFlow(FlowModel flow) => CurrentFlow = flow;

    [RelayCommand]
    private void AddFlow()
    {
        var flow = new FlowModel
        {
            Name = "New Flow",
        };
        Flows.Add(flow);
        CurrentFlow = flow;
    }
}