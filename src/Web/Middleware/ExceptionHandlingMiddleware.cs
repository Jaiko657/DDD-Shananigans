using System.Net;
using System.Text.Json;
using Core.Abstractions;
using Core.Abstractions.Exceptions;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private const int SqlUniqueConstraintViolation    = 2627;
    private const int SqlDuplicateKeyError            = 2601;
    private const int SqlForeignKeyViolation          = 547;

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var status = HttpStatusCode.InternalServerError;
        var code = "internal.error";
        var message = "An unexpected error occurred.";

        switch (exception)
        {
            case ValidationException fv:
                status = HttpStatusCode.BadRequest;
                code = "validation.failed";
                message = string.Join("; ", fv.Errors.Select(e => e.ErrorMessage));
                break;

            case DomainException de:
                status = HttpStatusCode.UnprocessableEntity;
                code = de.ErrorCode.ToLowerInvariant();
                message = de.Message;
                break;

            case DbUpdateException { InnerException: SqlException sqlEx }:
                switch (sqlEx.Number)
                {
                    case SqlUniqueConstraintViolation:
                    case SqlDuplicateKeyError:
                        status = HttpStatusCode.Conflict;
                        code = "database.duplicatekey";
                        message = "A record with the same key already exists.";
                        break;

                    case SqlForeignKeyViolation:
                        status = HttpStatusCode.UnprocessableEntity;
                        code = "database.foreignkeyviolation";
                        message = "Referenced entity does not exist.";
                        break;

                    default:
                        status = HttpStatusCode.UnprocessableEntity;
                        code = "database.error";
                        message = sqlEx.Message;
                        break;
                }
                break;

            case InvalidOperationException:
                status = HttpStatusCode.Conflict;
                code = "operation.invalid";
                message = exception.Message;
                break;

            case ArgumentException:
                status = HttpStatusCode.BadRequest;
                code = "argument.invalid";
                message = exception.Message;
                break;
        }

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(
            new ErrorResponse(code, message),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}