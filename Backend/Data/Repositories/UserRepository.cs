using backend.Helpers;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class UserRepository : IRepository<User>
{
    private readonly ApiDbContext _apiDbContext;
    public UserRepository(ApiDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }

    public async Task CreateAsync(User user)
    {
        _apiDbContext.Users.Add(user);
        await _apiDbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1)
    {
        return await _apiDbContext.Users.ToListAsync();
    }

    public async Task<PagedList<User>> GetManyAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1, SearchParameters parameters = null!)
    {
        var queryable = _apiDbContext.Users.AsQueryable().OrderBy(m => m.Username);
        return await PagedList<User>.CreateAsync(queryable, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<User?> GetAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1)
    {
        return await _apiDbContext.Users.FirstOrDefaultAsync(m => m.Id == userId);
    }

    public async Task UpdateAsync(User user)
    {
        _apiDbContext.Users.Update(user);
        await _apiDbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(User user)
    {
        _apiDbContext.Users.Remove(user);
        await _apiDbContext.SaveChangesAsync();
    }
}