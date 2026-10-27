using MdiWorkbench.Models;


namespace MdiWorkbench;

/// <summary>
/// In-memory sample implementation of <see cref="IProductDataService"/>. Seeds a small product
/// catalog and supports simple CRUD for the Data Grid document's edit demo.
/// </summary>
public class InMemoryProductDataService : IProductDataService
{
    private readonly List<Product> _products;
    private int _nextId;

    public InMemoryProductDataService()
    {
        _products = new List<Product>
        {
            NewProduct("Ultrabook ", "electronics-laptops", "Laptops", 1299.00m, 24),
            NewProduct("Gaming Laptop ", "electronics-laptops", "Laptops", 1899.00m, 12),
            NewProduct("Smartphone X", "electronics-phones", "Phones", 799.00m, 58),
            NewProduct("Smartphone Lite", "electronics-phones", "Phones", 399.00m, 91),
            NewProduct("Ergonomic Chair", "furniture-chairs", "Chairs", 249.00m, 30),
            NewProduct("Mesh Task Chair", "furniture-chairs", "Chairs", 179.00m, 45),
            NewProduct("Standing Desk", "furniture-desks", "Desks", 459.00m, 18),
            NewProduct("Compact Desk", "furniture-desks", "Desks", 219.00m, 27),
            NewProduct("A4 Paper Ream (500)", "stationery-paper", "Paper", 6.50m, 500),
            NewProduct("Recycled Notepad", "stationery-paper", "Paper", 3.25m, 320),
            NewProduct("Gel Pen 12-Pack", "stationery-pens", "Pens", 8.00m, 210),
            NewProduct("Fountain Pen", "stationery-pens", "Pens", 22.00m, 40),
        };
    }

    private Product NewProduct(string name, string categoryKey, string category, decimal price, int stock) =>
        new()
        {
            Id = ++_nextId,
            Name = name,
            CategoryKey = categoryKey,
            Category = category,
            Price = price,
            Stock = stock,
            UpdatedOn = DateTime.Today
        };

    public Task<List<Product>> GetProductsAsync(string? categoryKey = null)
    {
        IEnumerable<Product> query = _products;
        if (!string.IsNullOrWhiteSpace(categoryKey))
        {
            query = query.Where(p =>
                p.CategoryKey == categoryKey ||
                p.CategoryKey.StartsWith(categoryKey + "-", StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(query.OrderBy(p => p.Name).ToList());
    }

    public Task<Product> CreateAsync(Product product)
    {
        product.Id = ++_nextId;
        product.UpdatedOn = DateTime.Today;
        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<Product?> UpdateAsync(Product product)
    {
        var existing = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existing is null)
        {
            return Task.FromResult<Product?>(null);
        }

        existing.Name = product.Name;
        existing.CategoryKey = product.CategoryKey;
        existing.Category = product.Category;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        existing.UpdatedOn = DateTime.Today;
        return Task.FromResult<Product?>(existing);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var existing = _products.FirstOrDefault(p => p.Id == id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        _products.Remove(existing);
        return Task.FromResult(true);
    }
}
