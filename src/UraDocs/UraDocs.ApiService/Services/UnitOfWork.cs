using Microsoft.EntityFrameworkCore;
using UraDocs.ApiService.Data;

namespace UraDocs.ApiService.Services;

public class UnitOfWork
{
    private readonly UraDbContext _context;

    public UnitOfWork(UraDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> Query<T>() where T : class => _context.Set<T>();

    public async Task<bool> CommitAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task InsertAsync<T>(T entity) where T : class
    {
        _context.Add(entity);

        await CommitAsync();
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        _context.Update(entity);
        _context.Entry(entity).State = EntityState.Modified;
        await CommitAsync();
    }

    public async Task DeleteAsync<T>(T entity) where T : class
    {
        _context.Remove(entity);
        await CommitAsync();
    }
}
