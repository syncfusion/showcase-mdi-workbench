namespace MdiWorkbench.Models;

/// <summary>Headline KPI figures shown as dashboard cards.</summary>
public class AnalyticsSummary
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int ActiveUsers { get; set; }
    public double ConversionRatePercent { get; set; }

    // Per-KPI week-over-week deltas (positive = up, negative = down) for trend arrows.
    public double RevenueDeltaPercent { get; set; }
    public double OrdersDeltaPercent { get; set; }
    public double UsersDeltaPercent { get; set; }
    public double ConversionDeltaPercent { get; set; }
}

/// <summary>A single (category/period, value) point for chart series binding.</summary>
public class ChartPoint
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
}

/// <summary>
/// A single (category/period, value, secondaryValue) row for multi-series chart binding
/// (e.g. Revenue vs Orders, Visits vs Signups).
/// </summary>
public class ChartSeriesPoint
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
    public double SecondaryValue { get; set; }
}

/// <summary>
/// A single (label, value) row for accumulation (pie/doughnut) chart binding where the
/// legend label and slice name need to be exposed separately from the data label.
/// </summary>
public class CategorySharePoint
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
}

/// <summary>A single (label, value) row for ranking charts like Top Products bar charts.</summary>
public class RankingPoint
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
}

/// <summary>A single (label, units, revenue, change) row for the top-selling products table.</summary>
public class TopSellerRow
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int UnitsSold { get; set; }
    public decimal Revenue { get; set; }
    public double ChangePercent { get; set; }
}

/// <summary>A single (label, status, owner, amount) row for the recent activity table.</summary>
public class ActivityRow
{
    public string Time { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>A point on the rolling Live Sales line chart — one minute bucket.</summary>
public class LiveOrdersPoint
{
    public string MinuteLabel { get; set; } = string.Empty;
    public double Orders { get; set; }
}

/// <summary>A single recent order row for the Live Sales data grid.</summary>
public class LiveOrderRow
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime PlacedAt { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

/// <summary>One snapshot of the live sales report (chart series + top rows).</summary>
public class LiveSalesSnapshot
{
    public List<LiveOrdersPoint> OrdersPerMinute { get; set; } = new();
    public List<LiveOrderRow> LatestOrders { get; set; } = new();
    public DateTime CapturedAt { get; set; }
}
