using System;
using StarteR.Models.Steps;

namespace StarteR.StepManagement;

public record StepInfo(string TypeDiscriminator, string DisplayName, Type ModelType, Func<StepModelBase> Factory);
