using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Project;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.Task;

[Route("[controller]")]
[ApiController]
[Authorize]
public class TaskController(ITaskService taskService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] TaskRequest request, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        var task = await taskService.AddAsync(request, userId, ct);

        return task is null ? new BadRequestObjectResult("Task not found") : new CreatedResult("", task);

    }
}