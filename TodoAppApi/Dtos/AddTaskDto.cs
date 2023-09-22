namespace TodoAppApi.Dtos;

public class AddTaskDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime? DueTime { get; set; }
}