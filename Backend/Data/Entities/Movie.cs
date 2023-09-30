namespace backend.Data;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public Movie(int id, string title, string description, DateTime releaseDate)
    {
        Id = id;
        Title = title;
        Description = description;
        ReleaseDate = releaseDate;
    }
    public Movie()
    {
        Title = "";
        Description = "";
    }
}