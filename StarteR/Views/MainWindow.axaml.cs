using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;
using StarteR.Helpers;
using StarteR.ViewModels;

namespace StarteR.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Closing += OnClosing;

        MainNavigationView.Loaded += (_, _) =>
        {
            var menuItemsScrollViewer = MainNavigationView.FindDescendantOfType<ScrollViewer>(false, v => v.Name == "MenuItemsScrollViewer");
            if (menuItemsScrollViewer != null)
            {
                SmoothScrollHelper.For(menuItemsScrollViewer, Orientation.Vertical);
            }
        };
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.HandleWindowClosingAsync(e);
        }
    }
}