using FlowBoard.ListService.DTOs;
using FlowBoard.ListService.Interfaces;
using FlowBoard.Shared.DTOs;
using FlowBoard.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.ListService.Controllers;

[ApiController]
[Route("api/lists")]
[Authorize]
public class ListController : ControllerBase
{
    private readonly IListService _svc;
    public ListController(IListService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListDto dto)
    {
        try {
            var result = await _svc.CreateAsync(dto, User.GetUserId());
            return CreatedAtAction(nameof(GetById),
                new { id = result.ListId },
                ApiResponse<TaskListDto>.Ok(result, "List created"));
        }
        catch (Exception ex) {
            return BadRequest(ApiResponse<TaskListDto>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try {
            var result = await _svc.GetByIdAsync(id);
            return Ok(ApiResponse<TaskListDto>.Ok(result));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<TaskListDto>.Fail(ex.Message));
        }
    }

    [HttpGet("board/{boardId:int}")]
    public async Task<IActionResult> GetByBoard(int boardId)
    {
        var result = await _svc.GetByBoardIdAsync(boardId);
        return Ok(ApiResponse<List<TaskListDto>>.Ok(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateListDto dto)
    {
        try {
            var result = await _svc.UpdateAsync(id, dto);
            return Ok(ApiResponse<TaskListDto>.Ok(result, "List updated"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<TaskListDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}/move")]
    public async Task<IActionResult> Move(int id, [FromBody] MoveListDto dto)
    {
        try {
            var result = await _svc.MoveAsync(id, dto);
            return Ok(ApiResponse<TaskListDto>.Ok(result, "List moved"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<TaskListDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:int}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        try {
            await _svc.ArchiveAsync(id);
            return Ok(ApiResponse.OkNoData("List archived"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try {
            await _svc.DeleteAsync(id);
            return Ok(ApiResponse.OkNoData("List deleted"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }
}