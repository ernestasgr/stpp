using backend.Helpers;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class MovieRepository : IRepository<Movie>
{
    private readonly ApiDbContext _apiDbContext;
    public MovieRepository(ApiDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }

    public async Task CreateAsync(Movie movie)
    {
        _apiDbContext.Movies.Add(movie);
        await _apiDbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Movie>> GetAllAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1)
    {
        return await _apiDbContext.Movies.ToListAsync();
    }

    public async Task<PagedList<Movie>> GetManyAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1, SearchParameters parameters = null!)
    {
        var queryable = _apiDbContext.Movies.AsQueryable().OrderBy(m => m.Title);
        return await PagedList<Movie>.CreateAsync(queryable, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Movie?> GetAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1)
    {
        return await _apiDbContext.Movies.FirstOrDefaultAsync(m => m.Id == movieId);
    }

    public async Task UpdateAsync(Movie movie)
    {
        _apiDbContext.Movies.Update(movie);
        await _apiDbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Movie movie)
    {
        _apiDbContext.Movies.Remove(movie);
        await _apiDbContext.SaveChangesAsync();
    }
}