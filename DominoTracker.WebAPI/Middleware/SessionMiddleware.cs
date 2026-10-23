using DominoTracker.Application.Interfaces;
using DominoTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DominoTracker.WebAPI.Middleware;

public class SessionMiddleware
{
    private const string HeaderName = "X-Session-Id";

    private readonly RequestDelegate _next;

    public SessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentSessionAccessor currentSessionAccessor, AppDbContext dbContext)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var headerValue))
        {
            currentSessionAccessor.SessionId = 1;
            await _next(context);
            return;
        }

        if (!int.TryParse(headerValue, out var sessionId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"El header {HeaderName} debe ser un número entero");
            return;
        }

        var exists = await dbContext.Sessions.AnyAsync(s => s.Id == sessionId);

        if (!exists)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"No existe una sesión con id {sessionId}");
            return;
        }

        currentSessionAccessor.SessionId = sessionId;
        await _next(context);
    }
}
