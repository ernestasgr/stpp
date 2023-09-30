using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class ShowingRepository : IRepository<Showing>
{
    private readonly ApiDbContext _apiDbContext;
    public ShowingRepository(ApiDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }

    public async Task CreateAsync(Showing showing)
    {
        _apiDbContext.Showings.Add(showing);
        await _apiDbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Showing>> GetAllAsync(int movieId = -1, int showingId = -1, int ticketId = -1)
    {
        if(movieId >= 0)
        {
            return await _apiDbContext.Showings.Where(s => s.MovieId == movieId).ToListAsync();
        }
        return await _apiDbContext.Showings.ToListAsync();
    }

    public async Task<Showing?> GetAsync(int movieId = -1, int showingId = -1, int ticketId = -1)
    {
        return await _apiDbContext.Showings.Where(s => s.Number == showingId && s.MovieId == movieId).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Showing showing)
    {
        _apiDbContext.Showings.Update(showing);
        await _apiDbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Showing showing)
    {
        _apiDbContext.Showings.Remove(showing);
        await _apiDbContext.SaveChangesAsync();
    }
}