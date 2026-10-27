namespace MdiWorkbench.Models;

/// <summary>
/// A single node in the hierarchical navigation tree (used by the sidebar TreeView and the
/// Explorer document's left-hand tree pane). Self-referential via <see cref="ParentId"/> so it
/// binds directly to Syncfusion's TreeView field settings.
/// </summary>
public class CatalogNode
{
    public string Id { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IconCss { get; set; } = "e-icons e-folder";

    /// <summary>Optional category key used to filter product/grid data when this node is selected.</summary>
    public string? CategoryKey { get; set; }
}
