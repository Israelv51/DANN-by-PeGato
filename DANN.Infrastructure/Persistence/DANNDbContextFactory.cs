using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DANN.Infrastructure.Persistence;

public sealed class DANNDbContextFactory
    : IDesignTimeDbContextFactory<DANNDbContext>
{
    public DANNDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "DANN_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString =
                @"Server=(localdb)\MSSQLLocalDB;Database=DANN;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<DANNDbContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return new DANNDbContext(
            optionsBuilder.Options);
    }
}
