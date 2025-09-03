using Microsoft.FluentUI.AspNetCore.Components;

using Watch.Manager.ServiceDefaults;
using Watch.Manager.Web.Components;
using Watch.Manager.Web.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddWebServices();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.Services.AddFluentUIComponents();
builder.Services.AddHttpClient();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
