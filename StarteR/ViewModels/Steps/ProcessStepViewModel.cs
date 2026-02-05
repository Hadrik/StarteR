using CommunityToolkit.Mvvm.ComponentModel;
using StarteR.Models.Steps;

namespace StarteR.ViewModels.Steps;

public class ProcessStepViewModel : StepViewModelBase<ProcessStepModel>
{
    public ProcessStepViewModel(ProcessStepModel model) : base(model)
    {
    }
}