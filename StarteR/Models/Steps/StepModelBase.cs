using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StarteR.Models.Steps;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ProcessStepModel), "process")]
[JsonDerivedType(typeof(WebRequestStepModel), "web-request")]
public abstract partial class StepModelBase : ObservableObject
{
    public abstract StepType Type { get; }
    public abstract string DisplayName { get; }
    
    [ObservableProperty]
    private bool _isEnabled = true;
    
    [ObservableProperty]
    private bool _waitForCompletion = true;
    
    [ObservableProperty]
    private bool _isRunning;

    protected abstract Task ExecuteAsync();

    public async Task Run()
    {
        IsRunning = true;
        try
        {
            await ExecuteAsync();
        }
        finally
        {
            IsRunning = false;
        }
    }
}