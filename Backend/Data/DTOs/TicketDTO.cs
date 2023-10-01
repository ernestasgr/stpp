namespace backend.Data;

public record TicketCreateDTO(decimal Price, TicketType TicketType);
public record TicketUpdateDTO(TicketType TicketType);
public record TicketDTO(int Id, decimal Price, int MovieId, int ShowingNumber, TicketType TicketType);