using DANN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DANN.Infrastructure.Persistence;

public class DANNDbContext : DbContext
{
    public DANNDbContext(
        DbContextOptions<DANNDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectMember> ProjectMembers =>
        Set<ProjectMember>();

    public DbSet<TaskItem> TaskItems =>
        Set<TaskItem>();

    public DbSet<TaskDependency> TaskDependencies =>
        Set<TaskDependency>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DANNDbContext).Assembly);
    }
}
