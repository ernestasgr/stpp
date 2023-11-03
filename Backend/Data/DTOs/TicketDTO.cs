namespace backend.Data;

public record TicketCreateDTO(decimal Price, TicketType TicketType, string Seat);
public record TicketUpdateDTO(TicketType TicketType, string Seat);
public record TicketDTO(int Id, decimal Price, int MovieId, int ShowingNumber, TicketType TicketType, string Seat);