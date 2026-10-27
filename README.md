# showcase-mdi-workbench

A desktop-style **Multiple Document Interface (MDI)** shell built with **Syncfusion Blazor** components on **.NET 10**. Each "document" is a resizable, minimizable, maximizable, draggable window inside a single workspace — modeled on classic Windows MDI applications but rendered as a modern Blazor app.

The project uses Blazor's **Interactive Server** render mode (configured in [`Program.cs`](Program.cs) via `AddInteractiveServerComponents()` and `AddInteractiveServerRenderMode()`). It is **not** a WebAssembly project.

## Documents supported

Each document is hosted inside its own `SfDialog` pane inside [`Components/Mdi/MdiWorkspace.razor`](Components/Mdi/MdiWorkspace.razor). Window state (bounds, minimized, maximized, Z-order) is owned by [`MdiWindowManager`](Components/Mdi/MdiWindowManager.cs).

| Document | Source | Syncfusion component(s) |
|---|---|---|
| Explorer | [`Documents/ExplorerWindow.razor`](Components/Documents/ExplorerWindow.razor) | `SfSplitter`, `SfTreeView`, `SfGrid` |
| Data Grid | [`Documents/DataGridWindow.razor`](Components/Documents/DataGridWindow.razor) | `SfGrid` (sorting, filtering, paging, inline edit) |
| PDF Viewer | [`Documents/PdfViewerWindow.razor`](Components/Documents/PdfViewerWindow.razor) | `SfPdfViewer2` |
| Spreadsheet | [`Documents/SpreadsheetWindow.razor`](Components/Documents/SpreadsheetWindow.razor) | `SfSpreadsheet` |
| Dashboard | [`Documents/DashboardWindow.razor`](Components/Documents/DashboardWindow.razor) | `SfChart`, `SfAccumulationChart`, `SfGrid` |
| Live Sales Report | [`Documents/LiveSalesReportWindow.razor`](Components/Documents/LiveSalesReportWindow.razor) | `SfChart`, `SfGrid` |

## Window shell features

- **Drag** — header bar drag, configured via `AllowDragging` on `SfDialog`.
- **Resize** — `SfDialog` resize handles; new bounds are read back from the DOM via [`mdiWorkspace.getBounds`](wwwroot/mdi-interop.js) and pushed into the manager state.
- **Minimize** — hides the dialog via CSS without unmounting it; taskbar chip restores.
- **Maximize / Restore** — full-viewport expansion; toggle restores prior bounds.
- **Cascade / Tile / Close All** — invoked from the Window menu.
- **Single-instance windows** — Dashboard and Live Sales Report are marked `singleInstance: true`, so re-opening brings the existing window forward instead of creating a duplicate.
- **Loading overlay** — a workspace-level `SfSpinner` is shown from window creation through first render of the hosted document; it stays hidden during the artificial `Task.Delay(150)` so the user does not see a spinner for an idle wait.
- **Live viewport tracking** — a JS `ResizeObserver` keeps `MdiWindowManager.ViewportWidth/Height` in sync, so maximize always uses the current viewport size.
- **Component reflow** — after any size/position change, the dialog dispatches a resize notification so hosted Syncfusion components re-flow.

## Project layout

```
Components/
├── Layout/                # Top-level chrome: menu bar, navigation tree, main layout, reconnect modal
├── Mdi/                   # MdiWindow, MdiTaskbar, MdiWorkspace, MdiWindowManager
├── Documents/             # One file per hosted document type
└── Pages/                 # Home, Error, NotFound

Models/                    # MdiWindowState, MdiDocumentType, AnalyticsModels, CatalogNode, Product
Services/                  # InMemoryCatalogService, InMemoryProductDataService
wwwroot/                   # app.css, mdi-interop.js, bootstrap assets
Program.cs                 # Composition root — registers Syncfusion + Interactive Server
MdiWorkbench.csproj        # Syncfusion NuGet package references
```

## Syncfusion packages

Referenced in [`MdiWorkbench.csproj`](MdiWorkbench.csproj):

- `Syncfusion.Blazor.Buttons`
- `Syncfusion.Blazor.Charts`
- `Syncfusion.Blazor.Grid`
- `Syncfusion.Blazor.Layouts` (Splitter)
- `Syncfusion.Blazor.Navigations` (Menu, TreeView, ContextMenu)
- `Syncfusion.Blazor.Popups` (Dialog)
- `Syncfusion.Blazor.SfPdfViewer` (PDF Viewer 2)
- `Syncfusion.Blazor.Spreadsheet`
- `Syncfusion.Blazor.Spinner` (loading overlay, transitively used)
- `Syncfusion.Blazor.Themes` (Material 3 CSS, registered in [`App.razor`](Components/App.razor))

## Running locally

```powershell
dotnet build MdiWorkbench.slnx
dotnet run --project MdiWorkbench.csproj
```

The https profile launches at `https://localhost:7267` and `http://localhost:5250`. The http profile uses `http://localhost:5250`. Both are defined in [`Properties/launchSettings.json`](Properties/launchSettings.json). Use the **File** menu in the menu bar to open any document; use the **Window** menu to cascade/tile/close-all; use **Help → About MDI Workbench** to see the About dialog.

## License

Syncfusion Blazor components require a license key. This repository does not include one. Set the `SYNCFUSION_LICENSE_KEY` environment variable, or set `Syncfusion:LicenseKey` in configuration. The app registers whichever value is present at startup.
