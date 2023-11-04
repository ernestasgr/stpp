namespace backend.Data;

public record TicketCreateDTO(TicketType TicketType, string Seat);
public record TicketUpdateDTO(TicketType TicketType, string Seat);
public record TicketDTO(int Id, int MovieId, int ShowingNumber, TicketType TicketType, string Seat, string UserId);