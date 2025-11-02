using BlazorApp.Components;
using Serilog;
using Azure.Identity;
using Azure.Core;
using Microsoft.Azure.Cosmos; 


var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration["Cosmos:Key"];

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents() 
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<TokenCredential>(sp =>
{

    return new DefaultAzureCredential();
});

builder.Services.AddSingleton(sp =>
{
    var endpoint = "https://blazorexperimentcosmos.documents.azure.com:443/";

    var key = "key";

    return new CosmosClient(endpoint, key);

});
var app = builder.Build();   


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); 

app.UseStaticFiles(); 
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
 
app.Run();
