using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using WDA.Application;
using WDA.Infrastructure;
using WDA.Infrastructure.Persistence;
using WDA.WebApi.Extensions;

using var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddApplication();
builder.AddInfrastructure();

builder.AddWebServices();

var app = builder.Build();

// ConfigureBaseAuditableEntity the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
    
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.MapHealthChecks("/health");

    app.MapHealthChecks("/alive", new HealthCheckOptions
    {
        Predicate = r => r.Tags.Contains("live")
    });

    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();