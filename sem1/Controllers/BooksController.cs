
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public async Task<IActionResult> Index(string? sortOrder, string? currentFilter, string? searchString, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["TitleSortParm"] = sortOrder == "title" ? "title_desc" : "title";
        ViewData["PriceSortParm"] = sortOrder == "price" ? "price_desc" : "price";
        if (searchString != null) pageNumber = 1;
            else searchString = currentFilter;
        ViewData["CurrentFilter"] = searchString;
        var books = _context.Book.AsQueryable();
        if (!string.IsNullOrEmpty
            books = books.Where(b => b.Title.Contains(searchString));
        books = sortOrder switch
        {
            "title" => books.OrderBy(b => b.Title),
            "title_desc" => books.OrderByDescending(b => b.Title),
            "price" => books.OrderBy(b => b.Price),
            "price_desc" => books.OrderByDescending(b => b.Price),
            _ => books.OrderByDescending(b => b.Price) // [V2] TODO: înlocuiți cu sortarea implicită din varianta voastră / am inlocuit cu conform pret descresc
        };
        var rows = books.Select(b => new BookListItem
        .Select(b => new BookListItem(b.ID, b.Title,
        b.Author != null ? b.Author.FirstName + " " + b.Author.LastName : "",
        b.Genre != null ? b.Genre.Name : "",
        b.Price, b.Orders.Count))
        const int pageSize = 4; // NOU · [V1] TODO: înlocuiți 10 cu valoarea din varianta voastră
         return View(await PaginatedList<BookListItem>.CreateAsync(rows, pageNumber ?? 1, pageSize)); // NOU
    }
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
        ViewData["AuthorID"] = new SelectList(_context.Author.OrderBy(a => a.LastName), "ID",
            "FullName");
        ViewData["GenreID"] = new SelectList(_context.Genre.OrderBy(g => g.Name), "ID", "Name");
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

        ViewData["AuthorID"] = new SelectList(_context.Author.OrderBy(a => a.LastName), "ID",
            "FullName");
        ViewData["GenreID"] = new SelectList(_context.Genre.OrderBy(g => g.Name), "ID", "Name");

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
