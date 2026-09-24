using DANN.Domain.Entities;
using DANN.Domain.Enums;

namespace DANN.Domain.Services;

public static class GradeCalculator
{
    public static decimal CalculateEarnedPoints(
        IEnumerable<Assignment> assignments)
    {
        return Math.Round(
            assignments
                .Where(a => a.Status == AssignmentStatus.Graded)
                .Select(a => a.GetWeightedContribution() ?? 0)
                .Sum(),
            2);
    }

    public static decimal CalculateTotalWeight(
        IEnumerable<Assignment> assignments)
    {
        return Math.Round(
            assignments.Sum(a => a.WeightPercentage),
            2);
    }

    public static decimal CalculateGradedWeight(
        IEnumerable<Assignment> assignments)
    {
        return Math.Round(
            assignments
                .Where(a => a.Status == AssignmentStatus.Graded)
                .Sum(a => a.WeightPercentage),
            2);
    }

    public static decimal CalculateRemainingWeight(
        IEnumerable<Assignment> assignments)
    {
        var totalWeight = CalculateTotalWeight(assignments);
        var gradedWeight = CalculateGradedWeight(assignments);

        return Math.Max(0, totalWeight - gradedWeight);
    }

    public static decimal? CalculateRequiredAverage(
        IEnumerable<Assignment> assignments,
        decimal targetPercentage)
    {
        if (targetPercentage < 0 || targetPercentage > 100)
            throw new ArgumentOutOfRangeException(
                nameof(targetPercentage),
                "Target percentage must be between 0 and 100.");

        var earnedPoints = CalculateEarnedPoints(assignments);
        var remainingWeight = CalculateRemainingWeight(assignments);

        if (remainingWeight <= 0)
            return earnedPoints >= targetPercentage
                ? 0
                : null;

        var required =
            (targetPercentage - earnedPoints)
            / remainingWeight
            * 100;

        if (required <= 0)
            return 0;

        return Math.Round(required, 2);
    }

    public static bool IsTargetReachable(
        IEnumerable<Assignment> assignments,
        decimal targetPercentage)
    {
        var required =
            CalculateRequiredAverage(assignments, targetPercentage);

        return required is not null && required <= 100;
    }

    public static decimal ToTenPointScale(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage));

        return Math.Round(percentage / 10, 2);
    }
}
