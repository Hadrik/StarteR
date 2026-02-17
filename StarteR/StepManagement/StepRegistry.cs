using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StarteR.Models.Steps;

namespace StarteR.StepManagement;

public static class StepRegistry
{
    private static readonly Dictionary<Type, StepInfo> Steps = new();
    private static readonly Dictionary<string, StepInfo> StepsByDiscriminator = new();

    public static IReadOnlyCollection<StepInfo> AllSteps => Steps.Values;

    public static void Register<T>(string discriminator, string displayName) where T : StepModelBase, new()
    {
        var info = new StepInfo(discriminator, displayName, typeof(T), () => new T());
        Steps[typeof(T)] = info;
        StepsByDiscriminator[discriminator] = info;
    }

    public static StepInfo GetInfo<T>() where T : StepModelBase => Steps[typeof(T)];
    public static StepInfo GetInfo(Type type) => Steps[type];
    public static StepInfo? GetInfoByDiscriminator(string discriminator) =>
        StepsByDiscriminator.GetValueOrDefault(discriminator);

    public static StepModelBase Create(string discriminator)
    {
        if (!StepsByDiscriminator.TryGetValue(discriminator, out var info))
            throw new ArgumentException($"Unknown step type: {discriminator}", nameof(discriminator));
        return info.Factory();
    }

    public static StepModelBase Create(StepInfo info) => info.Factory();

    static StepRegistry()
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsSubclassOf(typeof(StepModelBase)) && !t.IsAbstract)
            .ToList().ForEach(t =>
        {
            var attr = t.GetCustomAttribute<RegisterStepAttribute>();
            if (attr == null) return;
            var method = typeof(StepRegistry).GetMethod(nameof(Register), BindingFlags.Public | BindingFlags.Static)
                ?.MakeGenericMethod(t);
            method?.Invoke(null, [ attr.Discriminator, attr.DisplayName ]);
        });
    }
}