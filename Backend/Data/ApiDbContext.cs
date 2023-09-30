using Microsoft.EntityFrameworkCore;
namespace backend.Data;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {

    }
    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<Showing> Showings { get; set; } = null!;
    public DbSet<Ticket> Tickets { get; set; } = null!;
}