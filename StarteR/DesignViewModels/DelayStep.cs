using System;
using StarteR.ViewModels.Steps;

namespace StarteR.DesignViewModels;

public class DelayStep() : DelayStepViewModel(new Models.Steps.DelayStepModel
{
    Delay = TimeSpan.FromSeconds(10).Add(TimeSpan.FromMinutes(5))
});