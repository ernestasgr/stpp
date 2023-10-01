using backend.Helpers;
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

    public async Task<PagedList<Ticket>> GetManyAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1, SearchParameters parameters = null!)
    {
        IOrderedQueryable<Ticket> queryable;
        if(movieId >= 0 && showingId == -1)
        {
            queryable = _apiDbContext.Tickets.AsQueryable().Where(
                t => t.MovieId == movieId).OrderBy(t => t.ShowingNumber);
        }
        else if(showingId >= 0)
        {
            queryable = _apiDbContext.Tickets.AsQueryable().Where(
                t => t.ShowingNumber == showingId && 
                t.MovieId == movieId).OrderBy(t => t.Id);
        }
        else
        {
            queryable = _apiDbContext.Tickets.AsQueryable().OrderBy(t => t.MovieId).ThenBy(t => t.ShowingNumber);
        }
        return await PagedList<Ticket>.CreateAsync(queryable, parameters.PageNumber, parameters.PageSize);
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