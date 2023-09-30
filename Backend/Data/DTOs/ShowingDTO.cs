namespace backend.Data;

public record ShowingCreateDTO(DateTime StartTime, DateTime EndTime);
public record ShowingDTO(int ID, int Number, DateTime StartTime, DateTime EndTime, int MovieId);