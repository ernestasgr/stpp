namespace backend.Data;

public class Ticket
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public Showing? Showing { get; set; }
    public string Seat { get; set; }
    public TicketType TicketType { get; set; }
    public Ticket(int id, decimal price, Showing showing, string seat, TicketType ticketType)
    {
        Id = id;
        Price = price;
        Showing = showing;
        Seat = seat;
        TicketType = ticketType;
    }
    public Ticket()
    {
        Seat = "";
    }
}

public enum TicketType
{
    Standard,
    Premium
}