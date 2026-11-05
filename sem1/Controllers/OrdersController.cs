using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sem1.Models;

public class OrdersController : Controller
{
    private readonly LibraryContext _context;

    public OrdersController(LibraryContext context)
    {
        _context = context;
    }

    // GET: Orders
    /*public async Task<IActionResult> Index(int? customerID)
    {
        var ordersQuery = _context.Order
            .Include(o => o.Customer)
            .Include(o => o.Book)
            .AsQueryable();

        // Filtrare după cititor (dropdown)
        if (customerID.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.CustomerID == customerID);
        }

        // Sortare descrescător după dată
        ordersQuery = ordersQuery.OrderByDescending(o => o.OrderDate);

        ViewData["CustomerID"] = new SelectList(_context.Customer, "ID", "Name", customerID);

        return View(await ordersQuery.ToListAsync());
    }*/
    /* public async Task<IActionResult> Index(int? customerID)
     {
         var ordersQuery = _context.Order
             .Include(o => o.Customer)
             .Include(o => o.Book)
             .AsQueryable();

         if (customerID.HasValue)
         {
             ordersQuery = ordersQuery.Where(o => o.CustomerID == customerID.Value);
         }

         ordersQuery = ordersQuery.OrderByDescending(o => o.OrderDate);

         ViewBag.CustomerID = new SelectList(
             await _context.Customer.ToListAsync(),
             "ID",
             "Name",
             customerID
         );

         return View(await ordersQuery.ToListAsync());
     }*/

    public async Task<IActionResult> Index(int? customerID)
    {
        var customers = await _context.Customer.ToListAsync();

        ViewBag.CustomerID = new SelectList(
            customers,
            "ID",
            "Name",
            customerID
        );

        var ordersQuery = _context.Order
            .Include(o => o.Customer)
            .Include(o => o.Book)
            .AsQueryable();

        if (customerID.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.CustomerID == customerID.Value);
        }

        ordersQuery = ordersQuery.OrderByDescending(o => o.OrderDate);

        return View(await ordersQuery.ToListAsync());
    }

    // GET: ORDERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Order
            .Include(o => o.Customer)
            .Include(o => o.Book)
            .FirstOrDefaultAsync(m => m.OrderID == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: ORDERS/Create
    public IActionResult Create()
    {
        ViewData["BookID"] = new SelectList(_context.Book, "ID", "Title");
        ViewData["CustomerID"] = new SelectList(_context.Customer, "ID", "Name");

        // Data implicită = data de azi
        var order = new Order { OrderDate = DateTime.Today };
        return View(order);
    }

    // POST: ORDERS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("OrderID,CustomerID,BookID,OrderDate")] Order order)
    {
        if (ModelState.IsValid)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["BookID"] = new SelectList(_context.Book, "ID", "Title", order.BookID);
        ViewData["CustomerID"] = new SelectList(_context.Customer, "ID", "Name", order.CustomerID);
        return View(order);
    }

    // GET: ORDERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Order.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        ViewData["BookID"] = new SelectList(_context.Book, "ID", "Title", order.BookID);
        ViewData["CustomerID"] = new SelectList(_context.Customer, "ID", "Name", order.CustomerID);
        return View(order);
    }

    // POST: ORDERS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("OrderID,CustomerID,BookID,OrderDate")] Order order)
    {
        if (id != order.OrderID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.OrderID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["BookID"] = new SelectList(_context.Book, "ID", "Title", order.BookID);
        ViewData["CustomerID"] = new SelectList(_context.Customer, "ID", "Name", order.CustomerID);
        return View(order);
    }

    // GET: ORDERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Order
            .Include(o => o.Customer)
            .Include(o => o.Book)
            .FirstOrDefaultAsync(m => m.OrderID == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: ORDERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var order = await _context.Order.FindAsync(id);
        if (order != null)
        {
            _context.Order.Remove(order);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OrderExists(int? id)
    {
        return _context.Order.Any(e => e.OrderID == id);
    }
}