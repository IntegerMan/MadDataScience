using MattEland.MadDataScience.Components;
using MattEland.MadDataScience.Models;
using MattEland.MadDataScience.Services;
using MudBlazor.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("MADDATASCIENCE_");

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddScoped<VisionService>();
builder.Services.AddScoped<SpeechService>();
builder.Services.AddScoped<MachineLearningService>();

builder.Services.Configure<AzureAiServicesConfig>(builder.Configuration.GetSection("AzureAiServices"));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();