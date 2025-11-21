using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private static List<string> tasks = new();

    // USER + ADMIN
    [HttpGet]
    [Authorize(Policy = "UserOrAdmin")]
    public IActionResult GetTasks() => Ok(tasks);
    [HttpPost]
    [Authorize(Policy = "UserOrAdmin")]
    public IActionResult AddTask(string task)
    {
        tasks.Add(task);
        return Ok("Task added");
    }

    [HttpDelete("{index}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Delete(int index)
    {
        if (index < 0 || index >= tasks.Count)
            return NotFound();

        tasks.RemoveAt(index);
        return Ok("Task deleted");
    }
}
