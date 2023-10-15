using System.ComponentModel.DataAnnotations;

namespace backend.Data;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Director { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<Showing> Showings { get; set; }
    [Required]
    public string UserId { get; set; }
    public User User { get; set; } = null!;
    public Movie(int id, string title, string description, DateTime releaseDate, string director, string userId)
    {
        Id = id;
        Title = title;
        Description = description;
        ReleaseDate = releaseDate;
        Showings = new List<Showing>();
        Director = director;
        UserId = userId;
    }
    public Movie()
    {
        Title = "";
        Description = "";
        Director = "";
        Showings = new List<Showing>();
        UserId = "";
    }
}