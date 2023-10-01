namespace backend.Data;

public record MovieCreateDTO(string Title, string Description, DateTime ReleaseDate, string Director);
public record MovieDTO(int Id, string Title, string Description, DateTime ReleaseDate, string Director);