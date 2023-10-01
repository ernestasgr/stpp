namespace backend.Data;

public class Showing
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
    public int Number { get; set; }
    public List<Ticket> Tickets { get; set; }
    public Showing(DateTime startTime, DateTime endTime, int movieId, Movie movie)
    {
        StartTime = startTime;
        EndTime = endTime;
        MovieId = movieId;
        Movie = movie;
        Tickets = new List<Ticket>();
    }
    public Showing()
    {
        Movie = null!;
        Tickets = new List<Ticket>();
    }
}