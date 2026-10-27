namespace MdiWorkbench.Models;

/// <summary>
/// Supplies the hierarchical navigation data shown in the sidebar TreeView and the
/// Explorer document's tree pane.
/// </summary>
public interface ICatalogService
{
    Task<List<CatalogNode>> GetCategoryTreeAsync();
}
