using System.ComponentModel.DataAnnotations;

namespace backend.Data;

public class Ticket : IUserOwnedResource
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public Showing Showing { get; set; }
    public string Seat { get; set; }
    public int ShowingNumber { get; set; }
    public int MovieId { get; set; }
    public TicketType TicketType { get; set; }
    [Required]
    public string UserId { get; set; }
    public User User { get; set; } = null!;
    public Ticket(int id, decimal price, Showing showing, TicketType ticketType, string userId, string seat)
    {
        Id = id;
        Price = price;
        Showing = showing;
        TicketType = ticketType;
        UserId = userId;
        Seat = seat;
    }
    public Ticket()
    {
        Showing = null!;
        UserId = "";
        Seat = "";
    }
}

public enum TicketType
{
    Standard,
    Premium
}