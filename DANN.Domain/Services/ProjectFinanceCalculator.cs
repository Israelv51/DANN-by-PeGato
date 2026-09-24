using DANN.Domain.Entities;
using DANN.Domain.Enums;

namespace DANN.Domain.Services;

public static class ProjectFinanceCalculator
{
    public static decimal CalculatePaidAmount(
        IEnumerable<Payment> payments)
    {
        return Math.Round(
            payments
                .Where(p => p.Status == PaymentStatus.Paid)
                .Sum(p => p.Amount),
            2);
    }

    public static decimal? CalculatePendingAmount(
        Project project,
        IEnumerable<Payment> payments)
    {
        if (project.AgreedAmount is null)
            return null;

        var paid = CalculatePaidAmount(payments);

        return Math.Max(
            0,
            Math.Round(project.AgreedAmount.Value - paid, 2));
    }

    public static decimal? CalculateCollectionPercentage(
        Project project,
        IEnumerable<Payment> payments)
    {
        if (project.AgreedAmount is null ||
            project.AgreedAmount <= 0)
        {
            return null;
        }

        var paid = CalculatePaidAmount(payments);

        return Math.Round(
            Math.Min(
                paid / project.AgreedAmount.Value * 100,
                100),
            2);
    }
}
