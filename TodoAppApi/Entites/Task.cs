namespace TodoAppApi.Entites;

public class Task
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedTime { get; set; }
    public bool Completed { get; set; }
    public DateTime? DueTime { get; set; }
}