using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Task = TodoAppApi.Entities.Task;

namespace TodoAppApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        try
        {
            var databaseCreator = Database.GetService<IRelationalDatabaseCreator>() as RelationalDatabaseCreator;
            if(databaseCreator != null)
            {
                if (!databaseCreator.CanConnect()) databaseCreator.Create();
                if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            //throw;
        }
    }
    public DbSet<Task> Tasks { get; set; }
}