using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using StarteR.Models.Steps;
using StarteR.ViewModels;

namespace StarteR.Views;

public partial class FlowEditorView : UserControl
{
    private readonly Symbol?[] _availableIcons = new Symbol?[] {null}.Concat(Enum.GetValues<Symbol>().Where(e =>
    {
        // Exclude icons with `ObsoleteAttribute`
        var memberInfo = typeof(Symbol).GetMember(e.ToString()).FirstOrDefault();
        return memberInfo == null || !Attribute.IsDefined(memberInfo, typeof(ObsoleteAttribute));
    }).Cast<Symbol?>()).ToArray();
    
    private readonly SolidColorBrush _hoverHighlightBrush = new SolidColorBrush(Colors.Pink);
    private IBrush? _defaultBorderBrush;
    private bool _isDragging;
    private StepModelBase? _draggedStep;
    private Border? _draggedBorder;
    private Border? _lastHoveredBorder;
    private int _targetIndex = -1;
    private Point _dragStartPosition;
    
    public FlowEditorView()
    {
        InitializeComponent();
        IconItemsRepeater.ItemsSource = _availableIcons;
        IconScrollViewer.PointerWheelChanged += (_, e) =>
        {
            if (e.Delta.Y > 0)
            {
                IconScrollViewer.PageLeft();
            }
            else
            {
                IconScrollViewer.PageRight();
            }
        };
    }

    private void StartNameEdit(object? _, TappedEventArgs e)
    {
        NameEditPanelView.IsVisible = false;
        NameEditPanelEdit.IsVisible = true;
        NameEditTextBox.Focus();
    }
    
    private void EndNameEdit(object? _, RoutedEventArgs e)
    {
        NameEditPanelView.IsVisible = true;
        NameEditPanelEdit.IsVisible = false;
    }
    
    private void OnDragHandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border dragHandle) return;
        
        // Find the parent Border (StepBorder) and get the StepModelBase from its DataContext
        var stepBorder = dragHandle.FindAncestorOfType<Border>();
        while (stepBorder != null && stepBorder.Name != "StepBorder")
        {
            stepBorder = stepBorder.FindAncestorOfType<Border>(true);
        }
        
        if (_defaultBorderBrush is null) 
            _defaultBorderBrush = stepBorder?.BorderBrush;
        
        if (stepBorder?.DataContext is StepModelBase step)
        {
            _isDragging = true;
            _draggedStep = step;
            _draggedBorder = stepBorder;
            _dragStartPosition = e.GetPosition(stepBorder);
            _targetIndex = -1;
            
            // Visual feedback - make it semi-transparent and bring to front
            stepBorder.Opacity = 0.6;
            stepBorder.ZIndex = 1000;
            stepBorder.RenderTransform = new TransformGroup
            {
                Children = [
                    new ScaleTransform(.9, .9),
                    new TranslateTransform()
                ]
            };
            stepBorder.IsHitTestVisible = false;
            e.Pointer.Capture(dragHandle);
        }
    }
    
    private void OnDragHandlePointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || _draggedStep == null || _draggedBorder == null) return;
        
        // Get the current pointer position relative to the dragged border's parent
        var parent = _draggedBorder.Parent as Control;
        if (parent == null) return;
        
        var currentPosition = e.GetPosition(parent);
        var offset = currentPosition.Y - _dragStartPosition.Y;
        
        // Translate dragged border
        (_draggedBorder.RenderTransform as TransformGroup)?.Children
            .OfType<TranslateTransform>()
            .FirstOrDefault()
            ?.SetValue(TranslateTransform.YProperty, offset);
        
        // Find which step we're hovering over to show visual feedback
        var hoveredBorder = GetStepBorderAtPosition(e.GetPosition(this));
        
        if (hoveredBorder != null && hoveredBorder != _draggedBorder && 
            hoveredBorder.DataContext is StepModelBase hoveredStep)
        {
            if (_lastHoveredBorder != hoveredBorder)
            {
                // Reset previous hovered border
                _lastHoveredBorder?.BorderBrush = _defaultBorderBrush;
                
                // Highlight new hovered border
                hoveredBorder.BorderBrush = _hoverHighlightBrush;
                
                _lastHoveredBorder = hoveredBorder;
            }
            
            if (DataContext is FlowEditorViewModel viewModel)
            {
                _targetIndex = viewModel.Model.Steps.IndexOf(hoveredStep);
            }
        }
    }
    
    private void OnDragHandlePointerReleased(object? _, PointerReleasedEventArgs e)
    {
        if (_isDragging && _draggedBorder != null && _draggedStep != null)
        {
            // Reset visual state
            _draggedBorder.Opacity = 1.0;
            _draggedBorder.ZIndex = 0;
            _draggedBorder.RenderTransform = null;
            _draggedBorder.IsHitTestVisible = true;
            e.Pointer.Capture(null);
            
            // Clear hovered border highlight
            if (_lastHoveredBorder != null)
            {
                _lastHoveredBorder.BorderBrush = _defaultBorderBrush;
                _lastHoveredBorder = null;
            }
            
            // Now perform the actual reordering in the collection
            if (_targetIndex != -1 && DataContext is FlowEditorViewModel viewModel)
            {
                var steps = viewModel.Model.Steps;
                var currentIndex = steps.IndexOf(_draggedStep);
                
                if (currentIndex != -1 && currentIndex != _targetIndex)
                {
                    steps.Move(currentIndex, _targetIndex);
                }
            }
        }
        
        _isDragging = false;
        _draggedStep = null;
        _draggedBorder = null;
        _targetIndex = -1;
    }
    
    private Border? GetStepBorderAtPosition(Point position)
    {
        var element = this.InputHitTest(position);
        
        if (element == null) return null;
        
        // Traverse up the visual tree to find a Border named "StepBorder"
        var current = element as Control;
        while (current != null)
        {
            if (current is Border { Name: "StepBorder" } border)
            {
                return border;
            }
            current = current.Parent as Control;
        }
        
        return null;
    }
}