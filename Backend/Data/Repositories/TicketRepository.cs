using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class TicketRepository : IRepository<Ticket>
{
    private readonly ApiDbContext _apiDbContext;
    public TicketRepository(ApiDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }

    public async Task CreateAsync(Ticket ticket)
    {
        _apiDbContext.Tickets.Add(ticket);
        await _apiDbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Ticket>> GetAllAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1)
    {
        if(movieId >= 0 && showingId == -1)
        {
            return await _apiDbContext.Tickets.Where(
                t => t.MovieId == movieId)
                .ToListAsync();
        }
        if(showingId >= 0)
        {
            return await _apiDbContext.Tickets.Where(
                t => t.ShowingNumber == showingId && 
                t.MovieId == movieId).ToListAsync();
        }
        return await _apiDbContext.Tickets.ToListAsync();
    }

    public async Task<Ticket?> GetAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1)
    {
        return await _apiDbContext.Tickets.Where(
            t => t.Id == ticketId && 
            t.ShowingNumber == showingId && 
            t.MovieId == movieId)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        _apiDbContext.Tickets.Update(ticket);
        await _apiDbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Ticket ticket)
    {
        _apiDbContext.Tickets.Remove(ticket);
        await _apiDbContext.SaveChangesAsync();
    }
}