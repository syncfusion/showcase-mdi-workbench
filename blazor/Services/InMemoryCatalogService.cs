

using MdiWorkbench.Models;

namespace MdiWorkbench;

/// <summary>
/// In-memory sample implementation of <see cref="ICatalogService"/>. Provides a small,
/// static product-category hierarchy used by the Explorer document's tree pane.
/// </summary>
public class InMemoryCatalogService : ICatalogService
{
    private static readonly List<CatalogNode> Nodes = new()
    {
        new CatalogNode { Id = "electronics", ParentId = null, Name = "Electronics", IconCss = "e-icons e-folder", CategoryKey = "electronics" },
        new CatalogNode { Id = "electronics-laptops", ParentId = "electronics", Name = "Laptops", IconCss = "e-icons e-file", CategoryKey = "electronics-laptops" },
        new CatalogNode { Id = "electronics-phones", ParentId = "electronics", Name = "Phones", IconCss = "e-icons e-file", CategoryKey = "electronics-phones" },

        new CatalogNode { Id = "furniture", ParentId = null, Name = "Furniture", IconCss = "e-icons e-folder", CategoryKey = "furniture" },
        new CatalogNode { Id = "furniture-chairs", ParentId = "furniture", Name = "Chairs", IconCss = "e-icons e-file", CategoryKey = "furniture-chairs" },
        new CatalogNode { Id = "furniture-desks", ParentId = "furniture", Name = "Desks", IconCss = "e-icons e-file", CategoryKey = "furniture-desks" },

        new CatalogNode { Id = "stationery", ParentId = null, Name = "Stationery", IconCss = "e-icons e-folder", CategoryKey = "stationery" },
        new CatalogNode { Id = "stationery-paper", ParentId = "stationery", Name = "Paper", IconCss = "e-icons e-file", CategoryKey = "stationery-paper" },
        new CatalogNode { Id = "stationery-pens", ParentId = "stationery", Name = "Pens", IconCss = "e-icons e-file", CategoryKey = "stationery-pens" },
    };

    public Task<List<CatalogNode>> GetCategoryTreeAsync() => Task.FromResult(Nodes.ToList());
}
