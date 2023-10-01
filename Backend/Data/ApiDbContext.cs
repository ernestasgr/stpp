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
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Showings)
            .WithOne(s => s.Movie)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<Showing>()
            .HasKey(s => new { s.MovieId, s.Number });

        modelBuilder.Entity<Showing>()
            .HasMany(s => s.Tickets)
            .WithOne(t => t.Showing)
            .HasForeignKey(t => new { t.MovieId, t.ShowingNumber})
            .HasPrincipalKey(s => new { s.MovieId, s.Number})
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ticket>()
            .HasKey(t => new { t.Id, t.MovieId, t.ShowingNumber });

        base.OnModelCreating(modelBuilder);
    }
}