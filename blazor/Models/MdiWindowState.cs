namespace MdiWorkbench.Models;

/// <summary>
/// Mutable state for a single MDI window: identity, hosted document, and bounds/visibility.
/// Instances are serialized to JSON for localStorage persistence, so all properties must have
/// public getters/setters and the type must have a public parameterless constructor.
/// </summary>
public class MdiWindowState
{
    /// <summary>Stable identifier for the window, used for lookup, focus, and close operations.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Text shown in the window's title bar and in the Window menu / taskbar chip.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Which document template to render inside the window.</summary>
    public MdiDocumentType DocumentType { get; set; }

    /// <summary>
    /// Free-form parameters passed to the hosted document component (e.g. a PDF file path,
    /// or a catalog category id to pre-filter an Explorer/Grid window). Kept as strings so the
    /// whole window state remains trivially JSON-serializable for persistence.
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new();

    public double Left { get; set; } = 40;
    public double Top { get; set; } = 40;
    public double Width { get; set; } = 640;
    public double Height { get; set; } = 440;

    /// <summary>Stacking order; higher values render above lower ones. Managed by MdiWindowManager.</summary>
    public int ZIndex { get; set; }

    public bool IsMinimized { get; set; }
    public bool IsMaximized { get; set; }

    /// <summary>Bounds saved before maximizing, restored when the window is un-maximized.</summary>
    public double RestoreLeft { get; set; }
    public double RestoreTop { get; set; }
    public double RestoreWidth { get; set; }
    public double RestoreHeight { get; set; }
}
