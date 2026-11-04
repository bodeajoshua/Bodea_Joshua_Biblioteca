
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sem1.Models;
using System.Diagnostics;

public class BooksController : Controller
{
    private readonly LibraryContext _context;
    private readonly ILogger<BooksController> _logger;

    public BooksController(
        LibraryContext context,
        ILogger<BooksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Books
    public async Task<IActionResult> Index()
    {
        var sw = Stopwatch.StartNew();

        var rows = await _context.Book
            .Select(b => new BookRow(
                b.Title,
                b.Author!.FirstName + " " + b.Author.LastName,
                b.Orders.Count
            ))
            .ToListAsync();

        _logger.LogInformation("Index: {Ms} ms", sw.ElapsedMilliseconds);

        return View(rows);
    }

    // GET: BOOKS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book
            .FirstOrDefaultAsync(m => m.ID == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: BOOKS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: BOOKS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Title,Author,Price,GenreID,Genre")] Book book)
    {
        if (ModelState.IsValid)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: BOOKS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var book = await _context.Book.FindAsync(id);

        if (book == null) return NotFound();

        var vm = new BookEditViewModel
        {
            ID = book.ID,
            Title = book.Title,
            Price = book.Price,
            AuthorID = book.AuthorID,
            GenreID = book.GenreID
        };

        return View(vm);
    }

    // POST: BOOKS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    // APARAREA 2 - ViewModel (varianta finala)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookEditViewModel vm)
    {
        if (id != vm.ID) return NotFound();

        if (!ModelState.IsValid) return View(vm);

        var book = await _context.Book.FindAsync(id);
        if (book is null) return NotFound();

        book.Title = vm.Title;
        book.Price = vm.Price;
        book.AuthorID = vm.AuthorID;
        book.GenreID = vm.GenreID;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    // GET: BOOKS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book
            .FirstOrDefaultAsync(m => m.ID == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: BOOKS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var book = await _context.Book.FindAsync(id);
        if (book != null)
        {
            _context.Book.Remove(book);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int? id)
    {
        return _context.Book.Any(e => e.ID == id);
    }
}
