using Core.Domain.Common;
using Core.Domain.Primatives;

namespace Core.Abstractions;

public interface IRepository<T> where T : Entity
{
    IQueryable<T> Query();
    Task<T?>       GetAsync(CancellationToken ct, params object[] key);
    Task           AddAsync(T entity, CancellationToken ct);
    void           Update(T entity);
    void           Remove(T entity);
}