using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Net.Http;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using StarteR.Models;
using StarteR.Models.Steps;
using StarteR.Services;
using StarteR.ViewModels;
using StarteR.Views;

namespace StarteR;

public class App : Application
{
    public Action Shutdown { get; private set; } = () => { };

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();
        _ = serviceProvider.GetRequiredService<SaveService>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            Shutdown = () => desktop.Shutdown();
            
            if (desktop.Args.Contains("--run"))
            {
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                var runner = serviceProvider.GetRequiredService<HeadlessRunnerService>();
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await runner.RunAsync();
                    desktop.Shutdown();
                }, DispatcherPriority.Background);
            }
            else
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(LoadModel());
        services.AddSingleton<SaveService>();
        services.AddSingleton<FlowRunnerService>();
        services.AddSingleton<HeadlessRunnerService>();
        services.AddSingleton<MainWindowViewModel>();
    }

    private AppModel LoadModel()
    {
        return ConfigManager.Load() ?? new AppModel
        {
            LoadedFromFile = false,
            Flows = [
                new FlowModel
                {
                    Name = "Sample Flow 1",
                    Steps = [
                        new ProcessStepModel
                        {
                            FilePath = "notepad.exe",
                            Arguments = "",
                            WorkingDirectory = ".",
                            IsEnabled = true
                        },
                        new ProcessStepModel
                        {
                            FilePath = "cmd.exe",
                            Arguments = "",
                            WorkingDirectory = ".",
                            IsEnabled = true
                        }
                    ]
                },
                new FlowModel
                {
                    Name = "Sample Flow 2",
                    Steps = [
                        new WebRequestStepModel
                        {
                            Url = "https://jsonplaceholder.typicode.com/posts",
                            Method = HttpMethod.Get,
                            BodyType = WebRequestBodyType.Text,
                            Body = "",
                            IsEnabled = true,
                            ErrorMessage = "Failed to make web request"
                        }
                    ]
                }
            ]
        };
    }
}