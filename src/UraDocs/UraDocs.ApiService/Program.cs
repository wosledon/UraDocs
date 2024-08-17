using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using UraDocs.ApiService.Data;
using UraDocs.ApiService.Extensions;
using UraDocs.ApiService.Services;
using UraDocs.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(builder =>
{
    builder.AddPolicy(Contas.GlobalCors, options =>
    {
        options.AllowAnyOrigin();
        options.AllowAnyMethod();
        options.AllowAnyHeader();
    });
});

builder.Services.AddRazorPages();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<MenuService>();
builder.Services.AddSingleton<DocumentService>();

builder.Services.AddSingleton<SnowflakeGeneratorService>();

// builder.Services.AddHostedService<MenuWatcherBackgroundService>();
builder.Services.AddHostedService<MarkdownWatcherBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseCors(Contas.GlobalCors);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UraDocs API V1");
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseBlazorFrameworkFiles();
app.MapRazorPages();

app.UseStaticFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.MapDefaultEndpoints();

app.Run();