using System.Security.Claims;
using Core.Interfaces.Users;


namespace Middlewares;
public class LogsMiddleware
{
    private readonly RequestDelegate _next;

    public LogsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILogService logsService, IUserService UserService)
    {
        // List of user IDs that will not generate logs
        List<int> usersToExcludeFromLogs = new() { 1 }; // Add the user IDs to exclude here

        var authClaims = context.User.FindFirst(ClaimTypes.Authentication);

        context.Request.EnableBuffering();
        string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;
        var ip = context.Connection.RemoteIpAddress?.ToString();
        string tipoConsulta;
        
        // Check if the current user ID is in the exclusion list
        bool excludeFromLogs = false;
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null)
        {
            int userId = int.Parse(userIdClaim.Value);
            if (usersToExcludeFromLogs.Contains(userId))
            {
                excludeFromLogs = true;
            }
        }

        Log log = new()
        {
            Fecha = DateTimeOffset.Now,
            IP = ip,
            Peticion = context.Request.Path,
            Contenido = (
                context.Request.Body == null || 
                context.Request.Path == "/users/login" || 
                context.Request.Path == "/users/register" || 
                context.Request.Path == "/users/refresh" ||
                context.Request.Path == "/users/editar" ||
                context.Request.Path == "/users/restaurar-password") ? "" : requestBody
        };

        if (!excludeFromLogs && context.Request.Method != "OPTIONS")
        {
            tipoConsulta = "JWT";
            var user = userIdClaim == null ? null : await UserService.FindDetailedById(int.Parse(userIdClaim.Value));
            log.TipoConsulta = tipoConsulta;
            log.UsuarioID = user?.Id;
            log.Mensaje = user == null ? $"Un usuario no autenticado solicitó {context.Request.Path}" : $"El usuario {user.Username} solicitó {context.Request.Path}";
            await logsService.AddLog(log);
            
        }

        await _next(context);
    }

}