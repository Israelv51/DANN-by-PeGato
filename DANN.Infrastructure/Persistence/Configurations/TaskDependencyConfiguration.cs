using DANN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DANN.Infrastructure.Persistence.Configurations;

public class TaskDependencyConfiguration
    : IEntityTypeConfiguration<TaskDependency>
{
    public void Configure(
        EntityTypeBuilder<TaskDependency> builder)
    {
        builder.ToTable("TaskDependencies");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.TaskItemId,
            x.DependsOnTaskItemId
        })
        .IsUnique();

        builder.HasOne<TaskItem>()
            .WithMany()
            .HasForeignKey(x => x.TaskItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TaskItem>()
            .WithMany()
            .HasForeignKey(x => x.DependsOnTaskItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
