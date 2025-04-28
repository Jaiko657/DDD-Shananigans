
using Infrastructure.DataAccess;
using MediatR;

namespace Infrastructure.Pipelines;

public sealed class UnitOfWorkBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes> where TReq : notnull
{
    private readonly AppDbContext _db;
    public UnitOfWorkBehavior(AppDbContext db) => _db = db;

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var response = await next(ct);     // run handler
        await _db.SaveChangesAsync(ct);         // commit EF changes
        await tx.CommitAsync(ct);               // commit DB tx
        return response;
    }
}