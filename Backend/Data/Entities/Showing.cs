using System.ComponentModel.DataAnnotations;

namespace backend.Data;

public class Showing
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
    public int Number { get; set; }
    public decimal Price { get; set; }
    public List<Ticket> Tickets { get; set; }
    [Required]
    public string UserId { get; set; }
    public User User { get; set; } = null!;
    public Showing(DateTime startTime, DateTime endTime, int movieId, Movie movie, string userId, decimal price)
    {
        StartTime = startTime;
        EndTime = endTime;
        MovieId = movieId;
        Movie = movie;
        Tickets = new List<Ticket>();
        UserId = userId;
        Price = price;
    }
    public Showing()
    {
        Movie = null!;
        Tickets = new List<Ticket>();
        UserId = "";
    }
}