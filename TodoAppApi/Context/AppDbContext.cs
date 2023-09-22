using Microsoft.EntityFrameworkCore;
using Task = TodoAppApi.Entites.Task;

namespace TodoAppApi.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<Task> Tasks { get; set; }
}