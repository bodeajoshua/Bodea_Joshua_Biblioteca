using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.ML;
using OpenAI;
using Scalar.AspNetCore;
using sem1.Api;
using sem1.Data;
using sem1.Models;
using System.ClientModel;
using sem1.Services;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("LibraryContext") ?? throw new InvalidOperationException("Connection string 'LibraryContext' not found.");

builder.Services.AddDbContext<LibraryContext>(options => options.UseSqlServer(connectionString));

// Înregistrăm o singură dată clientul Ollama ca IChatClient
var ollamaClient = new OllamaChatClient(
    new Uri("http://localhost:11434/"),
    modelId: "llama3"
);
builder.Services.AddSingleton(ollamaClient);
builder.Services.AddSingleton<IChatClient>(ollamaClient);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
builder.Services.AddScoped<RecommenderTrainer>();
builder.Services.AddPredictionEnginePool<BookText, GenrePrediction>()
 .FromFile(modelName: "GenreClassifier",
 filePath: "MLModels/GenreClassifier.zip", watchForChanges: true);
var app = builder.Build();
app.MapLibraryApi();

app.MapGet("/api/v2/readers/{id:int}/recommendations",
 async (int id, int? top, LibraryContext db,
 PredictionEnginePool<BookRating, BookRatingPrediction> pool) =>
 {
 var n = Math.Clamp(top ?? 5, 1, 20);
 var rated = await db.Rating.Where(r => r.ReaderId == id)
 .Select(r => r.BookID).ToListAsync();
 var candidates = await db.Book.AsNoTracking()
 .Where(b => !rated.Contains(b.ID))
 .Select(b => new { b.ID, b.Title }).ToListAsync();
 var recs = candidates
 .Select(b => new RecommendationDto(b.ID, b.Title,
 pool.Predict("BookRecommender",
 new BookRating { UserId = id, BookId = b.ID }).Score, false))
 .Where(r => !float.IsNaN(r.Score))
 .OrderByDescending(r => r.Score)
 .Take(n).ToList();
 if (recs.Count == 0) // cold start: cele mai bine evaluate cărți (minimum P evaluări, P = pragul variantei A)
 {
    var popular = await db.Rating
    .GroupBy(r => r.BookID)
    .Where(g => g.Count() >= 10) // înlocuiți 10 cu pragul variantei A
    .Select(g => new { BookID = g.Key, Avg = g.Average(r => r.Stars) })
    .OrderByDescending(x => x.Avg).Take(n)
    .ToListAsync();
    var ids = popular.Select(p => p.BookID).ToList();
    var titles = await db.Book.Where(b => ids.Contains(b.ID))
    .ToDictionaryAsync(b => b.ID, b => b.Title);
    recs = popular.Select(p => new RecommendationDto(
    p.BookID, titles[p.BookID], (float)p.Avg, true)).ToList();
}
     var log = new RecommendationLog
     {
         ReaderId = id,
         CreatedAt = DateTime.UtcNow,
         BookIds = string.Join(",", recs.Select(r => r.BookId))
     };

     db.RecommendationLog.Add(log);
     await db.SaveChangesAsync();
     return TypedResults.Ok(recs);
 })
.WithTags("Recomandări");

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


