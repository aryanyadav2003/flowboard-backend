using FlowBoard.CardService.DTOs;
using FlowBoard.CardService.Interfaces;
using FlowBoard.Shared.DTOs;
using FlowBoard.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.CardService.Controllers;

[ApiController]
[Route("api/cards")]
[Authorize]
public class CardController : ControllerBase
{
    private readonly ICardService _svc;
    public CardController(ICardService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCardDto dto)
    {
        try {
            var result = await _svc.CreateAsync(dto, User.GetUserId());
            return CreatedAtAction(nameof(GetById),
                new { id = result.CardId },
                ApiResponse<CardDto>.Ok(result, "Card created"));
        }
        catch (Exception ex) {
            return BadRequest(ApiResponse<CardDto>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try {
            var result = await _svc.GetByIdAsync(id);
            return Ok(ApiResponse<CardDto>.Ok(result));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<CardDto>.Fail(ex.Message));
        }
    }

    [HttpGet("list/{listId:int}")]
    public async Task<IActionResult> GetByList(int listId)
    {
        var result = await _svc.GetByListIdAsync(listId);
        return Ok(ApiResponse<List<CardDto>>.Ok(result));
    }

    [HttpGet("board/{boardId:int}")]
    public async Task<IActionResult> GetByBoard(int boardId)
    {
        var result = await _svc.GetByBoardIdAsync(boardId);
        return Ok(ApiResponse<List<CardDto>>.Ok(result));
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        var result = await _svc.GetOverdueAsync();
        return Ok(ApiResponse<List<CardDto>>.Ok(result));
    }

    [HttpGet("my-tasks")]
    public async Task<IActionResult> GetMyTasks()
    {
        var result = await _svc.GetMyTasksAsync(User.GetUserId());
        return Ok(ApiResponse<List<CardDto>>.Ok(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCardDto dto)
    {
        try {
            var result = await _svc.UpdateAsync(id, dto, User.GetUserId());
            return Ok(ApiResponse<CardDto>.Ok(result, "Card updated"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<CardDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}/move")]
    public async Task<IActionResult> Move(int id, [FromBody] MoveCardDto dto)
    {
        try {
            var result = await _svc.MoveAsync(id, dto, User.GetUserId());
            return Ok(ApiResponse<CardDto>.Ok(result, "Card moved"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<CardDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        try {
            await _svc.ArchiveAsync(id, User.GetUserId());
            return Ok(ApiResponse.OkNoData("Card archived"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try {
            await _svc.DeleteAsync(id, User.GetUserId());
            return Ok(ApiResponse.OkNoData("Card deleted"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    // ── Assignees ─────────────────────────────────────────────

    [HttpPost("{id:int}/assign")]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignUserDto dto)
    {
        try {
            var result = await _svc.AssignUserAsync(id, dto, User.GetUserId());
            return Ok(ApiResponse<CardAssigneeDto>.Ok(result, "User assigned"));
        }
        catch (KeyNotFoundException ex)      {
            return NotFound(ApiResponse<CardAssigneeDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex) {
            return Conflict(ApiResponse<CardAssigneeDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}/unassign/{userId:int}")]
    public async Task<IActionResult> Unassign(int id, int userId)
    {
        try {
            await _svc.UnassignUserAsync(id, userId, User.GetUserId());
            return Ok(ApiResponse.OkNoData("User unassigned"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}/assignees")]
    public async Task<IActionResult> GetAssignees(int id)
    {
        try {
            var result = await _svc.GetAssigneesAsync(id);
            return Ok(ApiResponse<List<CardAssigneeDto>>.Ok(result));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<List<CardAssigneeDto>>.Fail(ex.Message));
        }
    }
}