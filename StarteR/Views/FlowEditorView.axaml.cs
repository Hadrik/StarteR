using System;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Transformation;
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

    private const double DragOpacity = 0.6;
    private const double DragScale = 0.9;
    
    private readonly SolidColorBrush _highlightBorderBrush = new(Colors.Pink);
    private IBrush? _defaultBorderBrush;
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
        
        var stepBorder = FindStepBorder(dragHandle);
        _defaultBorderBrush ??= stepBorder?.BorderBrush;
        if (stepBorder?.DataContext is not StepModelBase step) return;

        _draggedStep = step;
        _draggedBorder = stepBorder;
        _dragStartPosition = e.GetPosition(stepBorder);
        _targetIndex = -1;
            
        stepBorder.ZIndex = 1000;
        stepBorder.Opacity = DragOpacity;
        stepBorder.RenderTransform = TransformOperations.Parse($"scale({DragScale})");
        stepBorder.IsHitTestVisible = false;
        e.Pointer.Capture(dragHandle);
    }
    
    private void OnDragHandlePointerMoved(object? sender, PointerEventArgs e)
    {
        if (_draggedStep == null || _draggedBorder == null) return;
        if (_draggedBorder.Parent is not Control parent) return;
        
        var offset = e.GetPosition(parent).Y - _dragStartPosition.Y;
        _draggedBorder.RenderTransform = TransformOperations.Parse($"scale({DragScale}) translateY({offset}px)");
        
        var hoveredBorder = GetStepBorderAtPosition(e.GetPosition(this));
        
        if (hoveredBorder != null && hoveredBorder != _draggedBorder && 
            hoveredBorder.DataContext is StepModelBase hoveredStep)
        {
            if (_lastHoveredBorder != hoveredBorder)
            {
                _lastHoveredBorder?.BorderBrush = _defaultBorderBrush;
                hoveredBorder.BorderBrush = _highlightBorderBrush;
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
        if (_draggedBorder != null && _draggedStep != null)
        {
            _draggedBorder.ZIndex = 0;
            _draggedBorder.RenderTransform = null;
            _draggedBorder.IsHitTestVisible = true;
            e.Pointer.Capture(null);
            
            if (_lastHoveredBorder != null)
            {
                _lastHoveredBorder.BorderBrush = _defaultBorderBrush;
                _lastHoveredBorder = null;
            }
            
            if (_targetIndex != -1 && DataContext is FlowEditorViewModel viewModel)
            {
                var steps = viewModel.Model.Steps;
                var currentIndex = steps.IndexOf(_draggedStep);
                
                if (currentIndex != -1 && currentIndex != _targetIndex)
                {
                    var moveInfos = steps.Select(step =>
                    {
                        var border = StepItemsControl.ContainerFromItem(step)?.FindDescendantOfType<Border>();
                        return new MoveInfo
                        {
                            Step = step,
                            BeforeY = border?.TranslatePoint(new Point(-border.Margin.Left, -border.Margin.Top), this)?.Y ?? 0,
                            Border = border == _draggedBorder ? border : null
                        };
                    }).ToArray();
                    
                    steps.Move(currentIndex, _targetIndex);
                    StepItemsControl.UpdateLayout();
                    AnimateMove(moveInfos);
                }
            }
        }

        _draggedStep = null;
        _draggedBorder = null;
        _targetIndex = -1;
    }

    private void AnimateMove(MoveInfo[] moveInfos)
    {
        foreach (var item in moveInfos)
        {
            var container = StepItemsControl.ContainerFromItem(item.Step);
            var afterY = container?.TranslatePoint(new Point(0, 0), this)?.Y;
            if (afterY is null) continue;
            
            var border = container!.FindDescendantOfType<Border>();
            if (border == null) continue;

            var deltaY = item.BeforeY - afterY.Value;
            
            var savedTransitions = border.Transitions;
            border.Transitions = null;
            var operation = $"translateY({deltaY}px)";
            if (item.Border != null)
            {
                operation += $" scale({DragScale})";
                border.Opacity = DragOpacity;
            }
            border.RenderTransform = TransformOperations.Parse(operation);
            
            border.Transitions = savedTransitions;
            border.Opacity = 1.0;
            border.RenderTransform = null;
        }
    }
    
    private Border? GetStepBorderAtPosition(Point position)
    {
        return FindStepBorder(this.InputHitTest(position) as Control);
    }
    
    private static Border? FindStepBorder(Control? current)
    {
        while (current != null)
        {
            if (current is Border { Name: "StepBorder" } border)
                return border;
            current = current.Parent as Control;
        }
        return null;
    }
    
    private record MoveInfo
    {
        public required StepModelBase Step { get; init; }
        public required double BeforeY { get; init; }
        public Border? Border { get; init; }
    }
}