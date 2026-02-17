using System;

namespace StarteR.StepManagement;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RegisterStepAttribute(string discriminator, string displayName) : Attribute
{
    public string Discriminator { get; set; } = discriminator;
    public string DisplayName { get; set; } = displayName;
}