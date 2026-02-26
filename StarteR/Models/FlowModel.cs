using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using StarteR.Models.Steps;

namespace StarteR.Models;

public partial class FlowModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private bool _isEnabled = true;
    
    [ObservableProperty]
    private Symbol? _icon;

    [JsonIgnore]
    public int ErrorCount => Steps.Count(s => !string.IsNullOrEmpty(s.ErrorMessage));
    
    public ObservableCollection<StepModelBase> Steps
    {
        get;
        set
        {
            if (field == value) return;
            UnsubscribeFromSteps(field);
            SetProperty(ref field, value);
            SubscribeToSteps(field);
            OnPropertyChanged(nameof(ErrorCount));
        }
    } = [];

    public FlowModel()
    {
        Steps.CollectionChanged += OnStepsCollectionChanged;
    }
    
    private void OnStepsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (StepModelBase step in e.OldItems)
                step.PropertyChanged -= OnStepPropertyChanged;
        
        if (e.NewItems != null)
            foreach (StepModelBase step in e.NewItems)
                step.PropertyChanged += OnStepPropertyChanged;
        
        OnPropertyChanged(nameof(ErrorCount));
    }

    private void OnStepPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(StepModelBase.ErrorMessage))
            OnPropertyChanged(nameof(ErrorCount));
    }

    private void SubscribeToSteps(ObservableCollection<StepModelBase> steps)
    {
        steps.CollectionChanged += OnStepsCollectionChanged;
        foreach (var step in steps)
            step.PropertyChanged += OnStepPropertyChanged;
    }
    
    private void UnsubscribeFromSteps(ObservableCollection<StepModelBase> steps)
    {
        steps.CollectionChanged -= OnStepsCollectionChanged;
        foreach (var step in steps)
            step.PropertyChanged -= OnStepPropertyChanged;
    }
}