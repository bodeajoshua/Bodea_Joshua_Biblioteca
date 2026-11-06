using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sem1.Models;
using sem1.Services;

namespace sem1.Controllers
{
    public class AdminController(LibraryContext db, RecommenderTrainer trainer) : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.Ratings = await db.Rating.CountAsync();
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportRatings(IFormFile? file)
        {
            if (file is null || file.Length == 0)
            {
                TempData["Msg"] = "Alegeți un fișier CSV.";
                return RedirectToAction(nameof(Index));
            }
            var ratings = new List<Rating>();
            using var reader = new StreamReader(file.OpenReadStream());
            await reader.ReadLineAsync(); 
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var p = line.Split(',');
                if (p.Length < 3 || !int.TryParse(p[0], out var u) ||
                !int.TryParse(p[1], out var b) || !int.TryParse(p[2], out var s))
                    continue;
                ratings.Add(new Rating
                {
                    ReaderId = u,
                    BookID = b,
                    Stars = s,
                    CreatedAt =
               DateTime.UtcNow
                });
            }
            db.Rating.AddRange(ratings);
            await db.SaveChangesAsync();
            TempData["Msg"] = $"Au fost importate {ratings.Count} evaluări.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Retrain()
        {
            var (rmse, count) = await trainer.TrainAsync();
            TempData["Msg"] = $"Model reantrenat pe {count} evaluări. RMSE = {rmse:0.00}";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Log(int? readerId, DateTime? startDate, DateTime? endDate, string sortOrder, int? pageNumber)
        {
            ViewData["CurrentReader"] = readerId;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            var logsQuery = db.RecommendationLog.AsQueryable();

            // Filtrare dupa cititor
            if (readerId.HasValue)
            {
                logsQuery = logsQuery.Where(l => l.ReaderId == readerId.Value);
            }

            // Filtrare dupa interval de date
            if (startDate.HasValue)
            {
                logsQuery = logsQuery.Where(l => l.CreatedAt >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                var endDateTime = endDate.Value.AddDays(1);
                logsQuery = logsQuery.Where(l => l.CreatedAt < endDateTime);
            }

            // Sortare dupa dată
            logsQuery = sortOrder switch
            {
                "date_desc" => logsQuery.OrderByDescending(l => l.CreatedAt),
                _ => logsQuery.OrderBy(l => l.CreatedAt),
            };

            int pageSize = 10;
            return View(await PaginatedList<RecommendationLog>.CreateAsync(logsQuery.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Carduri statistice
            ViewBag.TotalRatings = await db.Rating.CountAsync();
            ViewBag.DistinctReaders = await db.Rating.Select(r => r.ReaderId).Distinct().CountAsync();
            ViewBag.AverageRating = ViewBag.TotalRatings > 0 ? await db.Rating.AverageAsync(r => r.Stars) : 0;

            // Grafic 1: Distribuția notelor (1-5 stele)
            var ratingDistribution = new int[5];
            for (int i = 1; i <= 5; i++)
            {
                ratingDistribution[i - 1] = await db.Rating.CountAsync(r => r.Stars == i);
            }
            ViewBag.RatingDistribution = ratingDistribution;

            // Grafic 2: Media evaluărilor pe gen (Varianta B0)
            var avgByGenre = await db.Rating
                .Include(r => r.Book)
                .Where(r => r.Book != null && r.Book.Genre != null)
                .GroupBy(r => r.Book.Genre)
                .Select(g => new { Genre = g.Key, Avg = g.Average(r => (double)r.Stars) })
                .ToListAsync();

            ViewBag.Genres = avgByGenre.Select(x => x.Genre).ToArray();
            ViewBag.GenreAvgs = avgByGenre.Select(x => Math.Round(x.Avg, 2)).ToArray();

            return View();
        }

    }
}
