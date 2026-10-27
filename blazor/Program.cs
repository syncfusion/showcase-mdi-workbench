using MdiWorkbench;
using MdiWorkbench.Components;
using MdiWorkbench.Components.Mdi;
using MdiWorkbench.Models;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// Register the Syncfusion Blazor client service (required for every Syncfusion component).
builder.Services.AddSyncfusionBlazor();

// Required by SfPdfViewer2 to cache document/rendering data client-side.
builder.Services.AddMemoryCache();
builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 102400000; });
// --- Application services ------------------------------------------------
// In-memory mock data services.
builder.Services.AddSingleton<ICatalogService, InMemoryCatalogService>();
builder.Services.AddSingleton<IProductDataService, InMemoryProductDataService>();

// MDI shell services.
builder.Services.AddScoped<MdiWindowManager>();

var app = builder.Build();

var syncfusionLicenseKey =
    Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY")
    ?? builder.Configuration["Syncfusion:LicenseKey"];

if (!string.IsNullOrWhiteSpace(syncfusionLicenseKey))
{
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncfusionLicenseKey);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
