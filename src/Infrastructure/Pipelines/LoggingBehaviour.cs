using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Pipelines;

public sealed class LoggingBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes> where TReq : notnull
{
    private readonly ILogger<LoggingBehavior<TReq, TRes>> _log;
    public LoggingBehavior(ILogger<LoggingBehavior<TReq, TRes>> log) => _log = log;

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        _log.LogInformation("Handling {Request}", typeof(TReq).Name);
        var response = await next(ct);
        _log.LogInformation("Handled  {Request}", typeof(TReq).Name);
        return response;
    }
}