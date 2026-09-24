using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;

namespace DANN.Application.Features.Tasks;

public class RemoveTaskDependencyHandler
{
    private readonly ITaskDependencyRepository _dependencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTaskDependencyHandler(
        ITaskDependencyRepository dependencyRepository,
        IUnitOfWork unitOfWork)
    {
        _dependencyRepository = dependencyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<bool>> HandleAsync(
        RemoveTaskDependencyCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OwnerUserId == Guid.Empty ||
            command.TaskItemId == Guid.Empty ||
            command.DependsOnTaskItemId == Guid.Empty)
        {
            return ApplicationResult<bool>.Failure(
                "Valid identifiers are required.");
        }

        var dependency = await _dependencyRepository.GetAsync(
            command.TaskItemId,
            command.DependsOnTaskItemId,
            cancellationToken);

        if (dependency is null ||
            dependency.OwnerUserId != command.OwnerUserId)
        {
            return ApplicationResult<bool>.Failure(
                "Dependency not found.");
        }

        _dependencyRepository.Remove(dependency);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<bool>.Success(true);
    }
}
