using static System.Text.RegularExpressions.Regex;

namespace WDA.WebApi.Extensions;

public class UserScopeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserScopeMiddleware> _logger;
    private const string _pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";

    public UserScopeMiddleware(RequestDelegate next, ILogger<UserScopeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            var user = context.User;
            
            var maskedEmail = MaskEmail(user.Identity?.Name ?? string.Empty);

            var subjectId = user.Claims.First(c => c.Type == "sub")?.Value;

            using (_logger.BeginScope("User:{user}, SubjectId:{subject}", maskedEmail, subjectId))
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }

    private static string MaskEmail(string email) => 
        Replace(email, _pattern, m => new string('*', m.Length));
}