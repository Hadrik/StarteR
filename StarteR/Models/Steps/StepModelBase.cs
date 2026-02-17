using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarteR.StepManagement;

namespace StarteR.Models.Steps;

public abstract partial class StepModelBase : ObservableObject
{
    [JsonIgnore]
    public string DisplayName => StepRegistry.GetInfo(GetType()).DisplayName;
    
    [ObservableProperty]
    private bool _isEnabled = true;
    
    [ObservableProperty]
    private bool _waitForCompletion = true;
    
    [ObservableProperty]
    private string? _errorMessage;
    
    [ObservableProperty]
    [property: JsonIgnore]
    private bool _isRunning;

    [RelayCommand]
    private void ClearError()
    {
        ErrorMessage = null;
    }
    
    protected abstract Task ExecuteAsync();

    public async Task Run()
    {
        IsRunning = true;
        try
        {
            await ExecuteAsync();
            ClearError();
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
        }
        finally
        {
            IsRunning = false;
        }
    }
}