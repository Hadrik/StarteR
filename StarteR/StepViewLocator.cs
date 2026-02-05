using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using StarteR.Models.Steps;

namespace StarteR;

public class StepViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data is null)
            return new TextBlock { Text = "No step data", Foreground = Brushes.Red };
        
        var modelType = data.GetType();
        var modelName = modelType.FullName!;
        
        var viewModelName = modelName
            .Replace("Model", "ViewModel");
        
        var viewModelType = Type.GetType(viewModelName);
        if (viewModelType == null)
            return new TextBlock { Text = $"No ViewModel found for '{viewModelName}'", Foreground = Brushes.Red };
        
        var viewModel = Activator.CreateInstance(viewModelType, data);
        
        var viewName = viewModelName
            .Replace("ViewModel", "View");
        
        var viewType = Type.GetType(viewName);
        if (viewType == null)
            return new TextBlock { Text = $"No View found for '{viewName}'", Foreground = Brushes.Red };
        
        var view = (Control)Activator.CreateInstance(viewType)!;
        view.DataContext = viewModel;
        return view;
    }

    public bool Match(object? data)
    {
        return data is StepModelBase;
    }
}