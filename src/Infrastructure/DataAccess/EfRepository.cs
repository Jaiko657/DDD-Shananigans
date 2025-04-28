using Core.Abstractions;
using Core.Domain.Common;
using Core.Domain.Primatives;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess;

public sealed class EfRepository<T> : IRepository<T> where T : Entity
{
    private readonly AppDbContext _db;

    public EfRepository(AppDbContext db) => _db = db;

    public IQueryable<T> Query() => _db.Set<T>().AsNoTracking();

    public Task<T?> GetAsync(CancellationToken ct, params object[] key)
        => _db.Set<T>().FindAsync(key, ct).AsTask();

    public Task AddAsync(T entity, CancellationToken ct) => _db.Set<T>().AddAsync(entity, ct).AsTask();

    public void Update(T entity) => _db.Set<T>().Update(entity);

    public void Remove(T entity) => _db.Set<T>().Remove(entity);
}