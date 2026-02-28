using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using StarteR.Helpers;
using StarteR.Models;
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
        
        DataContextChanged += (_, _) =>
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.AppModel.Flows.CollectionChanged += OnFlowsCollectionChanged;
                foreach (var flow in vm.AppModel.Flows)
                    flow.PropertyChanged += OnFlowPropertyChangedForBadge;
            }
        };
        
        MainNavigationView.Loaded += (_, _) =>
        {
            if (DataContext is MainWindowViewModel vm)
                foreach (var flow in vm.AppModel.Flows)
                    UpdateInfoBadgeForFlow(flow);
        };
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.HandleWindowClosingAsync(e);
        }
    }

    private void MenuFlyoutItem_OnEnableToggle(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not MenuFlyoutItem { DataContext: FlowModel flowModel })
            return;
        
        flowModel.IsEnabled = !flowModel.IsEnabled;
    }
    
    private void OnFlowsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (FlowModel flow in e.OldItems)
                flow.PropertyChanged -= OnFlowPropertyChangedForBadge;
        
        if (e.NewItems != null)
            foreach (FlowModel flow in e.NewItems)
                flow.PropertyChanged += OnFlowPropertyChangedForBadge;
    }
    
    private void OnFlowPropertyChangedForBadge(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not FlowModel flow)
            return;
        
        if (e.PropertyName is nameof(FlowModel.IsEnabled) or nameof(FlowModel.ErrorCount))
            UpdateInfoBadgeForFlow(flow);
    }
    
    private NavigationViewItem? FindNavItemForFlow(FlowModel flow)
    {
        return MainNavigationView
            .GetVisualDescendants()
            .OfType<NavigationViewItem>()
            .FirstOrDefault(nvi => nvi.DataContext == flow);
    }

    /// <summary>
    /// Creates and assigns a new InfoBadge to the NavigationViewItem for the given flow.
    /// A new instance is required each time because FluentAvalonia's InfoBadge can only
    /// switch between <c>Value</c>, <c>Dot</c>, and <c>Icon</c> when created.
    /// </summary>
    private void UpdateInfoBadgeForFlow(FlowModel flow)
    {
        var navItem = FindNavItemForFlow(flow);
        if (navItem == null)
            return;
        
        var isVisible = !flow.IsEnabled || flow.ErrorCount > 0;
        
        if (!isVisible)
        {
            navItem.InfoBadge = null;
            return;
        }
        
        var isDot = !flow.IsEnabled && flow.ErrorCount == 0;
        
        var badge = new InfoBadge
        {
            Value = isDot ? -1 : flow.ErrorCount
        };
        badge.Classes.Add(flow.IsEnabled ? "Critical" : "Informational");
        
        navItem.InfoBadge = badge;
    }
}