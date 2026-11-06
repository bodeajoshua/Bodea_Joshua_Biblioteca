using Microsoft.EntityFrameworkCore;
using sem1.Models;

public class LibraryContext(DbContextOptions<LibraryContext> options) : DbContext(options)
{
    public DbSet<sem1.Models.Book> Book { get; set; } = default!;
    public DbSet<Author> Author { get; set; } = default!;
    public DbSet<Genre> Genre { get; set; } = default!;
    public DbSet<Publisher> Publisher { get; set; } = default!;
    public DbSet<Customer> Customer { get; set; } = default!;
    public DbSet<Order> Order { get; set; } = default!;
    public DbSet<Rating> Rating { get; set; } = default!;
    public DbSet<RecommendationLog> RecommendationLog { get; set; } = default!;
}
