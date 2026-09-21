using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Tasks.Commands;
using TaskFlow.Application.Tasks.Queries;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
public class TasksController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand(projectId,request.Title);

        var taskId = await sender.Send(command,cancellationToken);

        return Ok(taskId);
    }

    [HttpGet]
    public async Task<IActionResult> GetByProject(Guid projectId, CancellationToken cancellationToken)
    {
        var query = new GetTasksByProjectQuery(projectId);

        var tasks = await sender.Send(query, cancellationToken);

        return Ok(tasks);
    }
}

public record CreateTaskRequest(string Title);