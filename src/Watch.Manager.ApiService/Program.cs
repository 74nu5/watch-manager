using Watch.Manager.ApiService.Extensions;
using Watch.Manager.Service.Analyse.Extensions;
using Watch.Manager.Service.Database.Extensions;
using Watch.Manager.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowLocalOrigin",
        builderCors => builderCors.WithOrigins("https://localhost:7020")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod());

    options.AddPolicy(
        "AllowAzureOrigin",
        builderCors => builderCors.WithOrigins("https://app-watch-manager-web-fmc7d9f9cfekg6e6.francecentral-01.azurewebsites.net")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod());
});

builder.Configuration.AddAnalyzeConfiguration();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);
builder.AddAnalyzeServices();
builder.AddDatabaseServices();

var app = builder.Build();

app.UseCors("AllowLocalOrigin");
app.UseCors("AllowAzureOrigin");

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapArticleEndpoints();
app.MapCategoryEndpoints();
app.MapClassificationEndpoints();

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.Run();
