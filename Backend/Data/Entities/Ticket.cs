namespace backend.Data;

public class Ticket
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public Showing Showing { get; set; }
    public int ShowingNumber { get; set; }
    public int MovieId { get; set; }
    public TicketType TicketType { get; set; }
    public Ticket(int id, decimal price, Showing showing, TicketType ticketType)
    {
        Id = id;
        Price = price;
        Showing = showing;
        TicketType = ticketType;
    }
    public Ticket()
    {
        Showing = null!;
    }
}

public enum TicketType
{
    Standard,
    Premium
}