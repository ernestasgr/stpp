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
    public string MainImage { get; set; }
    public List<string> Images { get; set; }
    public List<string> Videos { get; set; }
    public Movie(int id, string title, string description, DateTime releaseDate, string director, 
    string userId, string mainImage, List<string> images, List<string> videos)
    {
        Id = id;
        Title = title;
        Description = description;
        ReleaseDate = releaseDate;
        Showings = new List<Showing>();
        Director = director;
        UserId = userId;
        MainImage = mainImage;
        Images = images;
        Videos = videos;
    }
    public Movie()
    {
        Title = "";
        Description = "";
        Director = "";
        Showings = new List<Showing>();
        UserId = "";
        MainImage = "";
        Images = new List<string>();
        Videos = new List<string>();
    }
}