using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace backend.Data;

public interface IRepository<T>
{
    Task CreateAsync(T data);
    Task<IReadOnlyList<T>> GetAllAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1);
    Task<T?> GetAsync(int movieId = -1, int showingId = -1, int ticketId = -1, int userId = -1);
    Task RemoveAsync(T data);
    Task UpdateAsync(T data);
}