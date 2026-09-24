using DANN.Application;
using DANN.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString =
    builder.Configuration.GetConnectionString("DANN");

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=DANN;Trusted_Connection=True;TrustServerCertificate=True;";
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        application = "DANN",
        status = "Healthy",
        utc = DateTimeOffset.UtcNow
    });
});

app.Run();
