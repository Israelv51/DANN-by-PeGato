using DANN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DANN.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration
    : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(
        EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("TaskItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(3000);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.Priority)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(x => x.OwnerUserId);

        builder.HasIndex(x => x.ProjectId);

        builder.HasIndex(x => x.ParentTaskItemId);

        builder.HasIndex(x => x.AssigneeProjectMemberId);

        builder.HasIndex(x => x.DueDateUtc);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<TaskItem>()
            .WithMany()
            .HasForeignKey(x => x.ParentTaskItemId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<ProjectMember>()
            .WithMany()
            .HasForeignKey(x => x.AssigneeProjectMemberId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
