using DANN.Application;
using DANN.Application.Features.Projects;
using DANN.Domain.Enums;
using DANN.Infrastructure;
using DANN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var connectionString =
    Environment.GetEnvironmentVariable("DANN_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=DANN;Trusted_Connection=True;TrustServerCertificate=True;";
}

var services = new ServiceCollection();

// Ahora las dos capas se registran con una sola línea cada una.
services.AddApplication();
services.AddInfrastructure(connectionString);

using var serviceProvider =
    services.BuildServiceProvider();

var ownerUserId = Guid.NewGuid();
var projectId = Guid.Empty;

Console.WriteLine("=== DANN - PRUEBA DE APPLICATION DI ===");
Console.WriteLine();

// ----------------------------------------------------
// 1. CREAR PROYECTO MEDIANTE APPLICATION
// ----------------------------------------------------

using (var scope = serviceProvider.CreateScope())
{
    var handler =
        scope.ServiceProvider
            .GetRequiredService<CreateProjectHandler>();

    var command = new CreateProjectCommand(
        ownerUserId,
        "DANN AddApplication Test",
        "Proyecto creado usando registro automático de handlers.",
        ProjectType.Personal,
        null,
        null);

    var result =
        await handler.HandleAsync(command);

    if (!result.IsSuccess ||
        result.Value is null)
    {
        throw new Exception(
            $"ERROR al crear proyecto: {result.Error}");
    }

    projectId = result.Value.Id;

    Console.WriteLine("1. CreateProjectHandler resuelto automáticamente.");
    Console.WriteLine($"   ID: {result.Value.Id}");
    Console.WriteLine($"   Nombre: {result.Value.Name}");
}

// ----------------------------------------------------
// 2. RECUPERARLO CON OTRO HANDLER DE APPLICATION
// ----------------------------------------------------

using (var scope = serviceProvider.CreateScope())
{
    var handler =
        scope.ServiceProvider
            .GetRequiredService<GetProjectByIdHandler>();

    var query = new GetProjectByIdQuery(
        ownerUserId,
        projectId);

    var result =
        await handler.HandleAsync(query);

    if (!result.IsSuccess ||
        result.Value is null)
    {
        throw new Exception(
            $"ERROR al recuperar proyecto: {result.Error}");
    }

    if (result.Value.Id != projectId ||
        result.Value.OwnerUserId != ownerUserId)
    {
        throw new Exception(
            "ERROR: Los datos recuperados no coinciden.");
    }

    Console.WriteLine();
    Console.WriteLine("2. GetProjectByIdHandler resuelto automáticamente.");
    Console.WriteLine($"   Nombre recuperado: {result.Value.Name}");
}

// ----------------------------------------------------
// 3. COMPROBAR OTROS HANDLERS
// ----------------------------------------------------

using (var scope = serviceProvider.CreateScope())
{
    _ = scope.ServiceProvider
        .GetRequiredService<GetProjectProgressHandler>();

    _ = scope.ServiceProvider
        .GetRequiredService<GetProjectHealthHandler>();

    _ = scope.ServiceProvider
        .GetRequiredService<GetProjectExecutiveSummaryHandler>();

    Console.WriteLine();
    Console.WriteLine("3. Handlers avanzados también registrados.");
    Console.WriteLine("   Progress          OK");
    Console.WriteLine("   Health            OK");
    Console.WriteLine("   Executive Summary OK");
}

// ----------------------------------------------------
// 4. LIMPIEZA
// ----------------------------------------------------

using (var scope = serviceProvider.CreateScope())
{
    var context =
        scope.ServiceProvider
            .GetRequiredService<DANNDbContext>();

    var project =
        await context.Projects
            .FirstOrDefaultAsync(
                x => x.Id == projectId);

    if (project is not null)
    {
        context.Projects.Remove(project);
        await context.SaveChangesAsync();
    }

    Console.WriteLine();
    Console.WriteLine("4. Proyecto temporal eliminado.");
}

Console.WriteLine();
Console.WriteLine("========================================");
Console.WriteLine("ADDAPPLICATION FUNCIONA CORRECTAMENTE");
Console.WriteLine("Application + Infrastructure + SQL");
Console.WriteLine("========================================");
