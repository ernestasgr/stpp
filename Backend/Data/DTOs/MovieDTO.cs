namespace backend.Data;

public record MovieCreateDTO(string Title, string Description, DateTime ReleaseDate);
public record MovieDTO(int Id, string Title, string Description, DateTime ReleaseDate);