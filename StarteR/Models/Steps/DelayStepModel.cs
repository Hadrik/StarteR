using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using StarteR.StepManagement;

namespace StarteR.Models.Steps;

[RegisterStep("delay", "Delay")]
public partial class DelayStepModel : StepModelBase
{
    [ObservableProperty]
    private TimeSpan _delay = TimeSpan.FromSeconds(10);
    
    protected override Task ExecuteAsync()
    {
        return Task.Delay(Delay);
    }
}