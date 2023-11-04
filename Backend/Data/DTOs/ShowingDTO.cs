namespace backend.Data;

public record ShowingCreateDTO(DateTime StartTime, DateTime EndTime, decimal Price);
public record ShowingDTO(int Number, DateTime StartTime, DateTime EndTime, int MovieId, decimal Price);