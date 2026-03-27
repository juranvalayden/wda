namespace WDA.WebApi.Extensions;

public static class WebServicesExtensions
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddProblemDetails();
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();
    }
}