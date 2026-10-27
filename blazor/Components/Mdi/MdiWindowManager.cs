
using MdiWorkbench.Models;

namespace MdiWorkbench.Components.Mdi;

/// <summary>
/// Owns the set of currently open MDI windows and every window-lifecycle operation
/// (open/close/activate/minimize/maximize/cascade/tile). UI components subscribe to
/// <see cref="WindowsChanged"/> and re-render; this class holds no rendering logic itself.
/// </summary>
public class MdiWindowManager
{
    private const double CascadeOffset = 32;
    private const double DefaultWidth = 640;
    private const double DefaultHeight = 440;
    private const double TaskbarReservedHeight = 48;

    private readonly List<MdiWindowState> _windows = new();
    private int _nextZIndex = 100;
    private int _cascadeStep;
    private Guid? _pendingFirstRenderWindowId;
    private DateTime _pendingStartedAt;

    private static readonly TimeSpan PostOpenSpinnerMinDuration = TimeSpan.FromMilliseconds(600);
    private CancellationTokenSource? _pendingSpinnerCts;

    /// <summary>Raised whenever the window collection or any window's state changes.</summary>
    public event Action? WindowsChanged;

    /// <summary>All open windows (including minimized ones), in no particular order.</summary>
    public IReadOnlyList<MdiWindowState> Windows => _windows;

    public bool IsOpeningWindow => _pendingFirstRenderWindowId.HasValue;

    /// <summary>The id of the window whose first render is still pending, if any.</summary>
    public Guid? PendingFirstRenderWindowId => _pendingFirstRenderWindowId;

    /// <summary>The workspace viewport size, kept in sync by MdiWorkspace for tile/cascade math.</summary>
    public double ViewportWidth { get; set; } = 1024;
    public double ViewportHeight { get; set; } = 640;

    public MdiWindowState OpenWindow(
        MdiDocumentType documentType,
        string title,
        Dictionary<string, string>? parameters = null,
        bool singleInstance = false)
    {
        if (singleInstance)
        {
            var existing = _windows.FirstOrDefault(w => w.DocumentType == documentType);
            if (existing is not null)
            {
                existing.IsMinimized = false;
                Activate(existing.Id);
                return existing;
            }
        }

        var offset = (_cascadeStep++ % 8) * CascadeOffset;
        var window = new MdiWindowState
        {
            Title = title,
            DocumentType = documentType,
            Parameters = parameters ?? new(),
            Left = 24 + offset,
            Top = 24 + offset,
            Width = DefaultWidth,
            Height = DefaultHeight,
            ZIndex = ++_nextZIndex
        };

        _windows.Add(window);
        RaiseChanged();
        return window;
    }

    public async Task OpenWindowWithDelayAsync(
        MdiDocumentType documentType,
        string title,
        Dictionary<string, string>? parameters = null,
        bool singleInstance = false)
    {
        if (_pendingFirstRenderWindowId.HasValue)
        {
            return;
        }

        // Delay runs silently; no spinner is shown yet on purpose — see this method's XML doc.
        await Task.Delay(150);

        var window = OpenWindow(documentType, title, parameters, singleInstance);

        // From here on the spinner must be visible until the host document finishes mounting
        // AND at least PostOpenSpinnerMinDuration has elapsed (whichever happens later).
        _pendingSpinnerCts?.Cancel();
        _pendingSpinnerCts?.Dispose();
        _pendingSpinnerCts = new CancellationTokenSource();
        _pendingFirstRenderWindowId = window.Id;
        _pendingStartedAt = DateTime.UtcNow;
        RaiseChanged();

        // Schedule the upper-bound hide at startedAt + PostOpenSpinnerMinDuration. If the
        // render-complete signal arrives earlier we'll reschedule for the remaining time;
        // if it arrives later (or never) this timer clears the spinner anyway.
        _ = HideSpinnerAfterDelayAsync(window.Id, PostOpenSpinnerMinDuration, _pendingSpinnerCts.Token);
    }

    private async Task HideSpinnerAfterDelayAsync(Guid windowId, TimeSpan delay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(delay, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return; // Rescheduled by a render-complete signal that arrived first.
        }
        catch (ObjectDisposedException)
        {
            return;
        }

        if (_pendingFirstRenderWindowId == windowId)
        {
            _pendingFirstRenderWindowId = null;
            RaiseChanged();
        }
    }

    public void MarkFirstRenderComplete(Guid id)
    {
        if (_pendingFirstRenderWindowId != id)
        {
            return;
        }

        var elapsed = DateTime.UtcNow - _pendingStartedAt;
        if (elapsed >= PostOpenSpinnerMinDuration)
        {
            // Minimum duration already elapsed — hide now.
            _pendingSpinnerCts?.Cancel();
            _pendingFirstRenderWindowId = null;
            RaiseChanged();
            return;
        }

        // Minimum duration not yet reached: reschedule the fallback for the remaining time
        // so it clears the spinner as soon as the minimum is hit.
        _pendingSpinnerCts?.Cancel();
        _pendingSpinnerCts?.Dispose();
        _pendingSpinnerCts = new CancellationTokenSource();
        var remaining = PostOpenSpinnerMinDuration - elapsed;
        _ = HideSpinnerAfterDelayAsync(id, remaining, _pendingSpinnerCts.Token);
    }

    public void Close(Guid id)
    {
        var window = Find(id);
        if (window is null)
        {
            return;
        }

        _windows.Remove(window);

        // If the closed window is the one we're waiting on, clear the spinner state too.
        if (_pendingFirstRenderWindowId == id)
        {
            _pendingFirstRenderWindowId = null;
            _pendingSpinnerCts?.Cancel();
            _pendingSpinnerCts?.Dispose();
            _pendingSpinnerCts = null;
        }

        RaiseChanged();
    }

    public void CloseAll()
    {
        if (_windows.Count == 0)
        {
            return;
        }

        _windows.Clear();
        RaiseChanged();
    }

    /// <summary>Brings a window to front and un-minimizes it.</summary>
    public void Activate(Guid id)
    {
        var window = Find(id);
        if (window is null)
        {
            return;
        }

        // No-op when already restored and topmost — avoids a pointless re-render that can
        // swallow the click that triggered activation (taskbar chip, menu item, ...).
        if (!window.IsMinimized && window.ZIndex == _nextZIndex)
        {
            return;
        }

        window.IsMinimized = false;
        window.ZIndex = ++_nextZIndex;
        RaiseChanged();
    }

    public void Minimize(Guid id)
    {
        var window = Find(id);
        if (window is null)
        {
            return;
        }

        if (window.IsMaximized)
        {
            window.IsMaximized = false;
            window.Left = window.RestoreLeft;
            window.Top = window.RestoreTop;
            window.Width = window.RestoreWidth;
            window.Height = window.RestoreHeight;
        }

        window.IsMinimized = true;
        RaiseChanged();
    }

    public void ToggleMaximize(Guid id)
    {
        var window = Find(id);
        if (window is null)
        {
            return;
        }

        if (window.IsMaximized)
        {
            window.IsMaximized = false;
            window.Left = window.RestoreLeft;
            window.Top = window.RestoreTop;
            window.Width = window.RestoreWidth;
            window.Height = window.RestoreHeight;
        }
        else
        {
            window.RestoreLeft = window.Left;
            window.RestoreTop = window.Top;
            window.RestoreWidth = window.Width;
            window.RestoreHeight = window.Height;

            window.IsMaximized = true;
            window.IsMinimized = false;
            window.Left = 0;
            window.Top = 0;
            window.Width = Math.Max(ViewportWidth, 200);
            window.Height = Math.Max(ViewportHeight - TaskbarReservedHeight, 200);
        }

        window.ZIndex = ++_nextZIndex;
        RaiseChanged();
    }

    /// <summary>Updates a window's position/size, e.g. from SfDialog drag/resize callbacks.</summary>
    public void UpdateBounds(Guid id, double left, double top, double width, double height)
    {
        var window = Find(id);
        if (window is null)
        {
            return;
        }

        window.Left = left;
        window.Top = top;
        window.Width = width;
        window.Height = height;
        window.IsMaximized = false;
        RaiseChanged();
    }

    /// <summary>Arranges all non-minimized windows in a diagonal cascade from the top-left.</summary>
    public void CascadeAll()
    {
        var visible = _windows.Where(w => !w.IsMinimized).OrderBy(w => w.ZIndex).ToList();
        for (var i = 0; i < visible.Count; i++)
        {
            var window = visible[i];
            window.IsMaximized = false;
            window.Left = 24 + (i * CascadeOffset);
            window.Top = 24 + (i * CascadeOffset);
            window.Width = DefaultWidth;
            window.Height = DefaultHeight;
            window.ZIndex = ++_nextZIndex;
        }

        RaiseChanged();
    }

    /// <summary>Arranges all non-minimized windows in an even grid across the workspace.</summary>
    public void TileAll()
    {
        var visible = _windows.Where(w => !w.IsMinimized).ToList();
        if (visible.Count == 0)
        {
            return;
        }

        var columns = (int)Math.Ceiling(Math.Sqrt(visible.Count));
        var rows = (int)Math.Ceiling(visible.Count / (double)columns);
        var cellWidth = Math.Max(ViewportWidth / columns, 260);
        var cellHeight = Math.Max((ViewportHeight - TaskbarReservedHeight) / rows, 200);

        for (var i = 0; i < visible.Count; i++)
        {
            var window = visible[i];
            var col = i % columns;
            var row = i / columns;

            window.IsMaximized = false;
            window.Left = col * cellWidth;
            window.Top = row * cellHeight;
            window.Width = cellWidth - 8;
            window.Height = cellHeight - 8;
            window.ZIndex = ++_nextZIndex;
        }

        RaiseChanged();
    }

    /// <summary>Replaces the entire window collection, e.g. when restoring a persisted layout.</summary>
    public void LoadState(IEnumerable<MdiWindowState> windows)
    {
        _windows.Clear();
        _windows.AddRange(windows);
        _nextZIndex = _windows.Count == 0 ? 100 : Math.Max(100, _windows.Max(w => w.ZIndex) + 1);
        RaiseChanged();
    }

    /// <summary>Returns a snapshot of current window state suitable for persistence.</summary>
    public List<MdiWindowState> GetSnapshot() => _windows.ToList();

    private MdiWindowState? Find(Guid id) => _windows.FirstOrDefault(w => w.Id == id);

    private void RaiseChanged() => WindowsChanged?.Invoke();
}
