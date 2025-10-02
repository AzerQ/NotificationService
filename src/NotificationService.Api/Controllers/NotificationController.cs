using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;

namespace NotificationService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationCommandService _commandService;
    private readonly INotificationQueryService _queryService;

    public NotificationController(
        INotificationCommandService commandService,
        INotificationQueryService queryService)
    {
        _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
    }

    [HttpPost]
    public async Task<ActionResult<NotificationResponseDto>> CreateAsync([FromBody] NotificationRequestDto request)
    {
        var result = await _commandService.CreateNotificationAsync(request);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/send")]
    public async Task<IActionResult> SendAsync(Guid id)
    {
        await _commandService.SendNotificationAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NotificationResponseDto>> GetByIdAsync(Guid id)
    {
        var result = await _queryService.GetByIdAsync(id);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("by-user/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyCollection<NotificationResponseDto>>> GetByUserAsync(Guid userId)
    {
        var result = await _queryService.GetByUserAsync(userId);
        return Ok(result);
    }

    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IReadOnlyCollection<NotificationResponseDto>>> GetByStatusAsync(string status)
    {
        var result = await _queryService.GetByStatusAsync(status);
        return Ok(result);
    }
}
