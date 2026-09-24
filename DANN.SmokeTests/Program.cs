using DANN.Application;
using DANN.Application.Features.Projects;
using DANN.Application.Features.Tasks;
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

services.AddApplication();
services.AddInfrastructure(connectionString);

using var serviceProvider = services.BuildServiceProvider();

var ownerUserId = Guid.NewGuid();
var projectId = Guid.Empty;

Console.WriteLine("=== DANN - PRUEBA DE APPLICATION DI ===");
Console.WriteLine();

// 1. CREAR PROYECTO

using (var scope = serviceProvider.CreateScope())
{
    var handler = scope.ServiceProvider
        .GetRequiredService<CreateProjectHandler>();

    var command = new CreateProjectCommand(
        ownerUserId,
        "DANN AddApplication Test",
        "Proyecto creado usando registro automático de handlers.",
        ProjectType.Personal,
        null,
        null);

    var result = await handler.HandleAsync(command);

    if (!result.IsSuccess || result.Value is null)
    {
        throw new Exception(
            $"ERROR al crear proyecto: {result.Error}");
    }

    projectId = result.Value.Id;

    Console.WriteLine("1. CreateProjectHandler resuelto automáticamente.");
    Console.WriteLine($"   ID: {result.Value.Id}");
    Console.WriteLine($"   Nombre: {result.Value.Name}");
}

// 2. RECUPERAR PROYECTO

using (var scope = serviceProvider.CreateScope())
{
    var handler = scope.ServiceProvider
        .GetRequiredService<GetProjectByIdHandler>();

    var result = await handler.HandleAsync(
        new GetProjectByIdQuery(ownerUserId, projectId));

    if (!result.IsSuccess || result.Value is null)
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

// 3. COMPROBAR HANDLERS AVANZADOS

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

// 4. LIMPIAR PROYECTO TEMPORAL

using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<DANNDbContext>();

    var project = await context.Projects
        .FirstOrDefaultAsync(x => x.Id == projectId);

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

// 5. PRUEBA AUTOMATIZADA DE DEPENDENCIAS

Console.WriteLine();
Console.WriteLine("=== DANN - PRUEBA DE DEPENDENCIAS ===");

var testOwnerUserId = Guid.NewGuid();
var mainTaskId = Guid.Empty;
var prerequisiteTaskId = Guid.Empty;

try
{
    // Crear las dos tareas temporales.

    using (var scope = serviceProvider.CreateScope())
    {
        var handler = scope.ServiceProvider
            .GetRequiredService<CreateTaskItemHandler>();

        var prerequisite = await handler.HandleAsync(
            new CreateTaskItemCommand(
                testOwnerUserId,
                "Requisito temporal SmokeTest",
                "Debe completarse antes de la tarea principal.",
                TaskPriority.Normal,
                null,
                null));

        if (!prerequisite.IsSuccess || prerequisite.Value is null)
        {
            throw new Exception(
                $"ERROR al crear requisito: {prerequisite.Error}");
        }

        prerequisiteTaskId = prerequisite.Value.Id;

        var mainTask = await handler.HandleAsync(
            new CreateTaskItemCommand(
                testOwnerUserId,
                "Tarea principal temporal SmokeTest",
                "Depende de un requisito pendiente.",
                TaskPriority.Normal,
                null,
                null));

        if (!mainTask.IsSuccess || mainTask.Value is null)
        {
            throw new Exception(
                $"ERROR al crear tarea principal: {mainTask.Error}");
        }

        mainTaskId = mainTask.Value.Id;
    }

    Console.WriteLine("1. Dos tareas temporales creadas.");

    // Registrar la dependencia.

    using (var scope = serviceProvider.CreateScope())
    {
        var handler = scope.ServiceProvider
            .GetRequiredService<AddTaskDependencyHandler>();

        var result = await handler.HandleAsync(
            new AddTaskDependencyCommand(
                testOwnerUserId,
                mainTaskId,
                prerequisiteTaskId));

        if (!result.IsSuccess)
        {
            throw new Exception(
                $"ERROR al registrar dependencia: {result.Error}");
        }
    }

    Console.WriteLine("2. Dependencia registrada.");

    // La tarea principal debe rechazar Completed.

    using (var scope = serviceProvider.CreateScope())
    {
        var handler = scope.ServiceProvider
            .GetRequiredService<ChangeTaskItemStatusHandler>();

        var result = await handler.HandleAsync(
            new ChangeTaskItemStatusCommand(
                testOwnerUserId,
                mainTaskId,
                TaskItemStatus.Completed));

        if (result.IsSuccess)
        {
            throw new Exception(
                "ERROR: DANN permitió completar una tarea bloqueada.");
        }

        Console.WriteLine("3. Tarea bloqueada rechazada correctamente.");
        Console.WriteLine($"   Motivo: {result.Error}");
    }

    // Completar primero el requisito.

    using (var scope = serviceProvider.CreateScope())
    {
        var handler = scope.ServiceProvider
            .GetRequiredService<ChangeTaskItemStatusHandler>();

        var result = await handler.HandleAsync(
            new ChangeTaskItemStatusCommand(
                testOwnerUserId,
                prerequisiteTaskId,
                TaskItemStatus.Completed));

        if (!result.IsSuccess ||
            result.Value is null ||
            result.Value.Status != TaskItemStatus.Completed)
        {
            throw new Exception(
                $"ERROR al completar requisito: {result.Error}");
        }
    }

    Console.WriteLine("4. Requisito completado.");

    // Ahora la tarea principal debe poder completarse.

    using (var scope = serviceProvider.CreateScope())
    {
        var handler = scope.ServiceProvider
            .GetRequiredService<ChangeTaskItemStatusHandler>();

        var result = await handler.HandleAsync(
            new ChangeTaskItemStatusCommand(
                testOwnerUserId,
                mainTaskId,
                TaskItemStatus.Completed));

        if (!result.IsSuccess ||
            result.Value is null ||
            result.Value.Status != TaskItemStatus.Completed)
        {
            throw new Exception(
                $"ERROR al completar tarea principal: {result.Error}");
        }
    }

    Console.WriteLine("5. Tarea principal completada tras resolver su requisito.");
}
finally
{
    // Limpiar únicamente los registros de esta prueba.
    // Primero la dependencia; después las tareas.

    using var scope = serviceProvider.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<DANNDbContext>();

    var dependencies = await context.TaskDependencies
        .Where(d =>
            d.OwnerUserId == testOwnerUserId &&
            (d.TaskItemId == mainTaskId ||
             d.TaskItemId == prerequisiteTaskId ||
             d.DependsOnTaskItemId == mainTaskId ||
             d.DependsOnTaskItemId == prerequisiteTaskId))
        .ToListAsync();

    context.TaskDependencies.RemoveRange(dependencies);
    await context.SaveChangesAsync();

    var temporaryTasks = await context.TaskItems
        .Where(t =>
            t.OwnerUserId == testOwnerUserId &&
            (t.Id == mainTaskId ||
             t.Id == prerequisiteTaskId))
        .ToListAsync();

    context.TaskItems.RemoveRange(temporaryTasks);
    await context.SaveChangesAsync();

    Console.WriteLine("6. Dependencia y tareas temporales eliminadas.");
}

Console.WriteLine();
Console.WriteLine("========================================");
Console.WriteLine("PRUEBA DE DEPENDENCIAS COMPLETADA");
Console.WriteLine("========================================");
