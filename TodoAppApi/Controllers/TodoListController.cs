using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAppApi.Context;
using TodoAppApi.Dtos;
using Task = TodoAppApi.Entities.Task;

namespace TodoAppApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoListController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    public TodoListController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await _dbContext.Tasks.ToListAsync();

        return Ok(tasks);
    }

    [HttpGet("{id:Guid}", Name = "GetTask")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> AddTask(AddTaskDto request)
    {
        var task = new Task()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedTime = DateTime.Now,
            Completed = false,
            DueTime = request.DueTime
        };
        
        await _dbContext.Tasks.AddAsync(task);
        await _dbContext.SaveChangesAsync();

        //return Ok(task);
        return CreatedAtRoute(nameof(GetTask), new { id = task.Id }, task);
    }

}