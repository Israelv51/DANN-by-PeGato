using DANN.Application.Common;
using DANN.Application.DTOs;
using DANN.Application.Features.Tasks;

namespace DANN.Application.Features.Projects;

public class GetProjectExecutiveSummaryHandler
{
    private readonly GetProjectByIdHandler _getProjectHandler;
    private readonly GetProjectProgressHandler _progressHandler;
    private readonly GetProjectHealthHandler _healthHandler;
    private readonly GetTaskRecommendationsHandler _recommendationsHandler;

    public GetProjectExecutiveSummaryHandler(
        GetProjectByIdHandler getProjectHandler,
        GetProjectProgressHandler progressHandler,
        GetProjectHealthHandler healthHandler,
        GetTaskRecommendationsHandler recommendationsHandler)
    {
        _getProjectHandler = getProjectHandler;
        _progressHandler = progressHandler;
        _healthHandler = healthHandler;
        _recommendationsHandler = recommendationsHandler;
    }

    public async Task<ApplicationResult<ProjectExecutiveSummaryDto>> HandleAsync(
        GetProjectExecutiveSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty ||
            query.ProjectId == Guid.Empty)
        {
            return ApplicationResult<ProjectExecutiveSummaryDto>.Failure(
                "Valid user and project identifiers are required.");
        }

        var projectResult =
            await _getProjectHandler.HandleAsync(
                new GetProjectByIdQuery(
                    query.OwnerUserId,
                    query.ProjectId),
                cancellationToken);

        if (!projectResult.IsSuccess ||
            projectResult.Value is null)
        {
            return ApplicationResult<ProjectExecutiveSummaryDto>.Failure(
                projectResult.Error);
        }

        var progressResult =
            await _progressHandler.HandleAsync(
                new GetProjectProgressQuery(
                    query.OwnerUserId,
                    query.ProjectId),
                cancellationToken);

        if (!progressResult.IsSuccess ||
            progressResult.Value is null)
        {
            return ApplicationResult<ProjectExecutiveSummaryDto>.Failure(
                progressResult.Error);
        }

        var healthResult =
            await _healthHandler.HandleAsync(
                new GetProjectHealthQuery(
                    query.OwnerUserId,
                    query.ProjectId,
                    query.NowUtc),
                cancellationToken);

        if (!healthResult.IsSuccess ||
            healthResult.Value is null)
        {
            return ApplicationResult<ProjectExecutiveSummaryDto>.Failure(
                healthResult.Error);
        }

        var priorities =
            await _recommendationsHandler.HandleAsync(
                new GetTaskRecommendationsQuery(
                    query.OwnerUserId,
                    query.NowUtc,
                    query.PriorityLimit,
                    query.ProjectId),
                cancellationToken);

        var result = new ProjectExecutiveSummaryDto(
            projectResult.Value,
            progressResult.Value,
            healthResult.Value,
            priorities,
            query.NowUtc);

        return ApplicationResult<ProjectExecutiveSummaryDto>.Success(
            result);
    }
}
