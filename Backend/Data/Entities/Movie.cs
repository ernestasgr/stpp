namespace backend.Data;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Director { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<Showing> Showings { get; set; }
    public Movie(int id, string title, string description, DateTime releaseDate, string director)
    {
        Id = id;
        Title = title;
        Description = description;
        ReleaseDate = releaseDate;
        Showings = new List<Showing>();
        Director = director;
    }
    public Movie()
    {
        Title = "";
        Description = "";
        Director = "";
        Showings = new List<Showing>();
    }
}