namespace MdiWorkbench.Models;

/// <summary>
/// CRUD data access for <see cref="Product"/> rows used by the Data Grid and Explorer document
/// windows.
/// </summary>
public interface IProductDataService
{
    Task<List<Product>> GetProductsAsync(string? categoryKey = null);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}
