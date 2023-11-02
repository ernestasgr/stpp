namespace backend.Data;

public record MovieCreateDTO(string Title, string Description, DateTime ReleaseDate, string Director, string MainImage, List<string> Images, List<string> Videos);
public record MovieDTO(int Id, string Title, string Description, DateTime ReleaseDate, string Director, string MainImage, List<string> Images, List<string> Videos);