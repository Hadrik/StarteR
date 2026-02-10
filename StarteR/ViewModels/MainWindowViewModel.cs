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
    private readonly SaveService _saveService;

    [ObservableProperty]
    private AppModel _appModel;
    
    [ObservableProperty]
    private FlowModel? _currentFlow;

    [ObservableProperty]
    private FlowEditorViewModel? _currentEditor;

    public MainWindowViewModel(AppModel appModel, FlowRunnerService flowRunner, SaveService saveService)
    {
        _flowRunner = flowRunner;
        _saveService = saveService;
        AppModel = appModel;
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
        AppModel.Flows.Add(flow);
        CurrentFlow = AppModel.Flows.Last();
    }

    [RelayCommand]
    private void Save()
    {
        _saveService.Save();
    }
    
    private void RemoveFlow(FlowModel flow)
    {
        AppModel.Flows.Remove(flow);
        CurrentFlow = AppModel.Flows.FirstOrDefault();
    }
}