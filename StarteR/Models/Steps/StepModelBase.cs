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
    public bool IsEnabled { get; set; } = true;
    public bool WaitForCompletion { get; set; } = true;
    
    [ObservableProperty]
    private bool _isRunning;

    public abstract Task ExecuteAsync();
}