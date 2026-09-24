using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Contracts.Tasks;
using TaskFlow.Application.Tasks.Commands;
using TaskFlow.Application.Tasks.Queries;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
public class TasksController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(Guid projectId, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand(projectId, request.Title);

        var taskId = await sender.Send(command, cancellationToken);

        return Created($"/api/projects/{projectId}/tasks/{taskId}", taskId);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByProject(Guid projectId, CancellationToken cancellationToken)
    {
        var query = new GetTasksByProjectQuery(projectId);

        var tasks = await sender.Send(query, cancellationToken);

        var response = tasks.Select(task => new TaskResponse(task.Id, task.Title, task.Status));

        return Ok(response);
    }
}

public record CreateTaskRequest(string Title);