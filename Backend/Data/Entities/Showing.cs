namespace backend.Data;

public class Showing
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MovieId { get; set; }
    public int Number { get; set; }
    public Showing(int id, DateTime startTime, DateTime endTime, int movieId)
    {
        Id = id;
        StartTime = startTime;
        EndTime = endTime;
        MovieId = movieId;
    }
    public Showing()
    {

    }
}