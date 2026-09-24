using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Domain.Entities;

namespace DANN.Application.Features.Tasks;

public class AddTaskDependencyHandler
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly ITaskDependencyRepository _dependencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddTaskDependencyHandler(
        ITaskItemRepository taskRepository,
        ITaskDependencyRepository dependencyRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _dependencyRepository = dependencyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<Guid>> HandleAsync(
        AddTaskDependencyCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OwnerUserId == Guid.Empty ||
            command.TaskItemId == Guid.Empty ||
            command.DependsOnTaskItemId == Guid.Empty)
        {
            return ApplicationResult<Guid>.Failure(
                "Valid identifiers are required.");
        }

        if (command.TaskItemId == command.DependsOnTaskItemId)
        {
            return ApplicationResult<Guid>.Failure(
                "A task cannot depend on itself.");
        }

        var taskItem = await _taskRepository.GetByIdAsync(
            command.TaskItemId,
            cancellationToken);

        var dependencyTask = await _taskRepository.GetByIdAsync(
            command.DependsOnTaskItemId,
            cancellationToken);

        if (taskItem is null ||
            dependencyTask is null ||
            taskItem.OwnerUserId != command.OwnerUserId ||
            dependencyTask.OwnerUserId != command.OwnerUserId)
        {
            return ApplicationResult<Guid>.Failure(
                "One or more tasks were not found.");
        }

        if (taskItem.ProjectId != dependencyTask.ProjectId)
        {
            return ApplicationResult<Guid>.Failure(
                "Dependent tasks must belong to the same project.");
        }

        var alreadyExists =
            await _dependencyRepository.ExistsAsync(
                taskItem.Id,
                dependencyTask.Id,
                cancellationToken);

        if (alreadyExists)
        {
            return ApplicationResult<Guid>.Failure(
                "This dependency already exists.");
        }

        var createsCycle = await CreatesCircularDependencyAsync(
            taskItem.Id,
            dependencyTask.Id,
            command.OwnerUserId,
            cancellationToken);

        if (createsCycle)
        {
            return ApplicationResult<Guid>.Failure(
                "This dependency would create a circular dependency.");
        }

        var dependency = new TaskDependency(
            command.OwnerUserId,
            taskItem.Id,
            dependencyTask.Id);

        await _dependencyRepository.AddAsync(
            dependency,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<Guid>.Success(
            dependency.Id);
    }

    private async Task<bool> CreatesCircularDependencyAsync(
        Guid originalTaskId,
        Guid proposedDependencyTaskId,
        Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var visited = new HashSet<Guid>();
        var pending = new Stack<Guid>();

        pending.Push(proposedDependencyTaskId);

        while (pending.Count > 0)
        {
            var currentTaskId = pending.Pop();

            if (currentTaskId == originalTaskId)
                return true;

            if (!visited.Add(currentTaskId))
                continue;

            var dependencies =
                await _dependencyRepository.GetByTaskAsync(
                    currentTaskId,
                    cancellationToken);

            foreach (var dependency in dependencies)
            {
                if (dependency.OwnerUserId != ownerUserId)
                    continue;

                pending.Push(
                    dependency.DependsOnTaskItemId);
            }
        }

        return false;
    }
}
