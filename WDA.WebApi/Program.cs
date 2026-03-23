using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using WDA.Application;
using WDA.Infrastructure;
using WDA.Infrastructure.Persistence;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();

    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    //// Apply EF Core migrations on startup
    //using (var scope = app.Services.CreateScope())
    //{
    //    var db = scope.ServiceProvider.GetRequiredService<WdaDbContext>();
        
    //    var isCreated = db.Database.EnsureCreated();

    //    db.Database.Migrate();
    //}

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}