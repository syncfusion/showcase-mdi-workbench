namespace MdiWorkbench.Models;

/// <summary>Row model for the Data Grid and Explorer document windows.</summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryKey { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime UpdatedOn { get; set; }
}
