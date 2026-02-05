using CommunityToolkit.Mvvm.ComponentModel;
using StarteR.Models.Steps;

namespace StarteR.ViewModels.Steps;

public abstract class StepViewModelBase<TModel> : ObservableObject where TModel : StepModelBase
{
    public TModel Model { get; }
    
    protected StepViewModelBase(TModel model)
    {
        Model = model;
    }
}