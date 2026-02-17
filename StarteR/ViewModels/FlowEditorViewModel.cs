using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using StarteR.Models;
using StarteR.Models.Steps;
using StarteR.Services;
using StarteR.StepManagement;

namespace StarteR.ViewModels;

public partial class FlowEditorViewModel : ViewModelBase
{
    private readonly FlowRunnerService _flowRunner;
    private readonly Action<FlowModel> _removeFlowAction;

    [ObservableProperty]
    private FlowModel _model;
    
    [ObservableProperty]
    private StepModelBase? _selectedStep;
    
    public static IReadOnlyCollection<StepInfo> AvailableStepTypes { get; } = StepRegistry.AllSteps;

    public FlowEditorViewModel(FlowModel flow, FlowRunnerService flowRunner, Action<FlowModel> removeFlowAction)
    {
        Model = flow;
        _flowRunner = flowRunner;
        _removeFlowAction = removeFlowAction;
    }

    [RelayCommand]
    private void AddStep(StepInfo stepInfo)
    {
        var step = StepRegistry.Create(stepInfo);
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
    
    [RelayCommand]
    private void SetIcon(Symbol? icon)
    {
        Model.Icon = icon;
    }
}