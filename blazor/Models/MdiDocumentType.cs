namespace MdiWorkbench.Models;
/// <summary>
/// Identifies which document/component template an <see cref="MdiWindowState"/> hosts.
/// Adding a new value here plus a matching case in the workspace's document renderer
/// is the extension point for future document types (Kanban, Scheduler, etc.).
/// </summary>
public enum MdiDocumentType
{
    Explorer,
    DataGrid,
    PdfViewer,
    Spreadsheet,
    Dashboard,
    LiveSalesReport
}
