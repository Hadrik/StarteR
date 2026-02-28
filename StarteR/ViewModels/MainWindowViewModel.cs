using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
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
    private object? _currentSelection;

    [ObservableProperty]
    private ViewModelBase? _currentEditor;

    public MainWindowViewModel(AppModel appModel, FlowRunnerService flowRunner, SaveService saveService)
    {
        _flowRunner = flowRunner;
        _saveService = saveService;
        AppModel = appModel;
    }

    partial void OnCurrentSelectionChanged(object? value)
    {
        CurrentEditor = value switch
        {
            FlowModel flow => new FlowEditorViewModel(flow, _flowRunner, RemoveFlow),
            NavigationViewItem { Name: "SettingsItem" } => new SettingsViewModel(),
            _ => null
        };
    }

    [RelayCommand]
    private void AddFlow()
    {
        var flow = new FlowModel
        {
            Name = "New Flow",
        };
        AppModel.Flows.Add(flow);
        CurrentSelection = AppModel.Flows.Last();
    }

    [RelayCommand]
    private void Save()
    {
        _saveService.Save();
    }
    
    [RelayCommand]
    private void RemoveFlow(FlowModel flow)
    {
        AppModel.Flows.Remove(flow);
        CurrentSelection = AppModel.Flows.FirstOrDefault();
    }

    public async Task HandleWindowClosingAsync(WindowClosingEventArgs e)
    {
        if (!_saveService.HasUnsavedChanges()) return;
        e.Cancel = true;

        var dialog = new ContentDialog
        {
            Title = "Unsaved Changes",
            Content = "You have unsaved changes. Do you want to save before exiting?",
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Don't Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary
        };

        var result = await dialog.ShowAsync();
        switch (result)
        {
            case ContentDialogResult.Primary:
                // Save and exit
                _saveService.Save();
                (Application.Current as App)?.Shutdown();
                break;
            case ContentDialogResult.Secondary:
                // Exit without saving
                (Application.Current as App)?.Shutdown();
                break;
            case ContentDialogResult.None:
                // Cancel - do nothing, window stays open
                break;
        }
    }
}