using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarteR.Models;
using StarteR.Models.Steps;
using StarteR.Services;

namespace StarteR.ViewModels;

public partial class FlowEditorViewModel : ViewModelBase
{
    private readonly FlowRunnerService _flowRunner;
    private readonly Action<FlowModel> _removeFlowAction;

    [ObservableProperty]
    private FlowModel _model;
    
    [ObservableProperty]
    private StepModelBase? _selectedStep;
    
    public static IReadOnlyList<StepType> AvailableStepTypes { get; } = Enum.GetValues<StepType>();

    public FlowEditorViewModel(FlowModel flow, FlowRunnerService flowRunner, Action<FlowModel> removeFlowAction)
    {
        Model = flow;
        _flowRunner = flowRunner;
        _removeFlowAction = removeFlowAction;
    }

    [RelayCommand]
    private void AddStep(StepType type)
    {
        var step = CreateStep(type);
        Model.Steps.Add(step);
    }

    [RelayCommand]
    private void RemoveStep(StepModelBase step)
    {
        Model.Steps.Remove(step);
    }
    
    [RelayCommand]
    private async Task RunFlow()
    {
        await _flowRunner.RunAsync(Model);
    }

    [RelayCommand]
    private void RemoveThisFlow()
    {
        _removeFlowAction(Model);
    }

    private static StepModelBase CreateStep(StepType type) => type switch
    {
        StepType.Process => new ProcessStepModel(),
        StepType.WebRequest => new WebRequestStepModel(),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}