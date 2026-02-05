using System;

namespace StarteR.Models.Steps;

public record StepTypeDescriptor(
    string TypeId,
    string DisplayName,
    Type ModelType,
    Type ExecutorType,
    Type ViewModelType
);