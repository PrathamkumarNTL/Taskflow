
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Api.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception ex,
        CancellationToken ct)
    {
        var status = ex switch
        {
            ValidationException =>
                StatusCodes.Status400BadRequest,

            KeyNotFoundException =>
                StatusCodes.Status404NotFound,

            UnauthorizedAccessException =>
                StatusCodes.Status403Forbidden,

            DbUpdateConcurrencyException =>
                StatusCodes.Status409Conflict,

            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = status,
            Title = status switch
            {
                400 => "Validation failed",
                404 => "Resource not found",
                403 => "Forbidden",
                409 => "Concurrency conflict",
                _ => "Unexpected server error"
            },
            Detail = status == 500 ? null : ex.Message,
            Instance = context.Request.Path
        };

        if (ex is ValidationException validation)
        {
            problem.Extensions["errors"] = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());
        }

        await System.Text.Json.JsonSerializer.SerializeAsync(context.Response.Body,problem,cancellationToken: ct);

        return true;
    }
}
