using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ML;
using sem1.Api;
using sem1.Data;
using sem1.Models;
using Scalar.AspNetCore;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("LibraryContext") ?? throw new InvalidOperationException("Connection string 'LibraryContext' not found.");

builder.Services.AddDbContext<LibraryContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services
    .AddPredictionEnginePool<BookRating, BookRatingPrediction>()
    .FromFile(
        modelName: "BookRecommender",
        filePath: "MLModels/BookRecommender.zip",
        watchForChanges: true);
builder.Services.AddValidation();   // validare automată pentru Minimal API
builder.Services.AddOpenApi();
var app = builder.Build();
app.MapLibraryApi();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    db.Database.Migrate();
    LibrarySeed.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();               // /openapi/v1.json (OpenAPI 3.1 în .NET 10)
    app.MapScalarApiReference();    // /scalar
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "start",
    pattern: "start",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
