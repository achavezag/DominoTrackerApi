using System.Security.Claims;
using DominoTracker.Application.Constants;
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
        var role = context.User.FindFirstValue(ClaimTypes.Role);

        // Usuarios autenticados que no son super_admin: la sesión activa es
        // siempre la que viene fija en el JWT (claim "sessionId"); se ignora
        // cualquier X-Session-Id que mande el cliente, para que no pueda
        // "entrar" a una sesión distinta a la suya.
        if (context.User.Identity?.IsAuthenticated == true && role != RoleNames.SuperAdmin)
        {
            var sessionIdClaim = context.User.FindFirstValue("sessionId");

            if (int.TryParse(sessionIdClaim, out var claimSessionId))
            {
                currentSessionAccessor.SessionId = claimSessionId;
            }

            await _next(context);
            return;
        }

        // super_admin (o requests sin autenticar, p.ej. /api/auth/login):
        // comportamiento original basado en header, default sesión 1.
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
