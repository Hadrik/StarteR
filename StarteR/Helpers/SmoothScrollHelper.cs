using System;
using System.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace StarteR.Helpers;

/// <remarks>
/// App crashes when inspecting ScrollViewer's `Offset` property in Legacy DevTools.
/// </remarks>
public class SmoothScrollHelper
{
    private const long Duration = 170;
    private const double StepSize = 100;

    private double _finishPosition;
    private readonly VectorTransition _transition;
    private readonly ScrollViewer _sv;
    private readonly Orientation _orientation;
    private bool _isAttached;

    public SmoothScrollHelper(ScrollViewer sv, Orientation orientation)
    {
        _sv = sv;
        _orientation = orientation;
        sv.Transitions ??= [];
        _transition = GetOrCreateTransition();

        sv.AddHandler(InputElement.PointerWheelChangedEvent, (sender, e) =>
        {
            if (sender is not ScrollViewer scrollViewer) return;

            _finishPosition += e.Delta.Y > 0 ? -StepSize : StepSize;
            _finishPosition = orientation == Orientation.Horizontal
                ? Math.Clamp(_finishPosition, 0, scrollViewer.Extent.Width - scrollViewer.Viewport.Width)
                : Math.Clamp(_finishPosition, 0, scrollViewer.Extent.Height - scrollViewer.Viewport.Height);
            scrollViewer.Offset = VecWithValue(_finishPosition);

            // Don't let ScrollViewer handle the event
            e.Handled = true;
        }, RoutingStrategies.Tunnel);

        // Smooth scroll can be created after the template is applied
        sv.TemplateApplied += (_, _) =>
        {
            if (!_isAttached)
            {
                OnScrollbarsCreated();
                _isAttached = true;
            }
        };
        sv.Loaded += (_, _) =>
        {
            if (!_isAttached)
            {
                OnScrollbarsCreated();
                _isAttached = true;
            }
        };

        // Must reset scroll position, otherwise it's random.
        // Sometimes it resets automatically, and sometimes it doesn't.
        // If it does, `_finishPosition` is wrong.
        sv.DetachedFromVisualTree += (_, _) =>
        {
            DisableAnimations();
            sv.Offset = VecWithValue(0);
            _finishPosition = 0;
        };
        sv.AttachedToVisualTree += (_, _) =>
        {
            EnableAnimations();
            sv.Offset = VecWithValue(0);
            _finishPosition = 0;
        };
    }

    public static SmoothScrollHelper For(ScrollViewer sv, Orientation orientation) => new(sv, orientation);

    private void EnableAnimations()
    {
        if (_sv.Transitions is null || _sv.Transitions.Any(e => e is VectorTransition)) return;
        _sv.Transitions.Add(_transition);
    }

    private void DisableAnimations()
    {
        _sv.Transitions?.Remove(_transition);
    }

    private void OnScrollbarsCreated()
    {
        var sb = _sv.FindDescendantOfType<ScrollBar>(false, e => e.Name == (
            _orientation == Orientation.Horizontal
                ? "PART_HorizontalScrollBar"
                : "PART_VerticalScrollBar"));

        EnableAnimations();

        if (sb != null)
            AttachScrollbarHandlers(sb);
    }

    private void AttachScrollbarHandlers(ScrollBar scrollBar)
    {
        scrollBar.AddHandler(InputElement.PointerPressedEvent, (_, _) => { DisableAnimations(); },
            RoutingStrategies.Bubble, true);

        scrollBar.AddHandler(InputElement.PointerReleasedEvent, (sender, e) =>
        {
            if (sender is not ScrollBar scroll) return;
            _finishPosition = scroll.Value;
            // ScrollBar does set OffsetProperty Value but not its EffectiveValue.
            // When moving the scrollbar from edge to position where scrolling will hit the same edge offset won't get updated.
            // The value gets compared with EffectiveValue and decides that it didn't change so it doesn't raise PropertyChanged.
            // I don't really understand why it works like this, but setting it again fixes it.
            _sv.Offset = VecWithValue(scroll.Value);
            EnableAnimations();
        }, RoutingStrategies.Bubble, true);
    }

    private VectorTransition GetOrCreateTransition()
    {
        var existingTransition = _sv.Transitions?.OfType<VectorTransition>().FirstOrDefault();
        if (existingTransition != null)
        {
            return existingTransition;
        }

        return new VectorTransition
        {
            Property = ScrollViewer.OffsetProperty,
            Duration = TimeSpan.FromMilliseconds(Duration),
            Easing = new CubicEaseOut()
        };
    }

    private Vector VecWithValue(double value) => _orientation == Orientation.Horizontal
        ? new Vector(value, _sv.Offset.Y)
        : new Vector(_sv.Offset.X, value);
}