using FlowBoard.ChecklistService.DTOs;
using FlowBoard.ChecklistService.Interfaces;
using FlowBoard.Shared.DTOs;
using FlowBoard.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.ChecklistService.Controllers;

[ApiController]
[Route("api/checklists")]
[Authorize]
public class ChecklistController : ControllerBase
{
    private readonly IChecklistService _svc;
    public ChecklistController(IChecklistService svc) => _svc = svc;

    // ── Checklist CRUD ────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChecklistDto dto)
    {
        try {
            var result = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                new { id = result.ChecklistId },
                ApiResponse<ChecklistDto>.Ok(result, "Checklist created"));
        }
        catch (Exception ex) {
            return BadRequest(ApiResponse<ChecklistDto>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try {
            var result = await _svc.GetByIdAsync(id);
            return Ok(ApiResponse<ChecklistDto>.Ok(result));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<ChecklistDto>.Fail(ex.Message));
        }
    }

    [HttpGet("card/{cardId:int}")]
    public async Task<IActionResult> GetByCard(int cardId)
    {
        var result = await _svc.GetByCardIdAsync(cardId);
        return Ok(ApiResponse<List<ChecklistDto>>.Ok(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, [FromBody] UpdateChecklistDto dto)
    {
        try {
            var result = await _svc.UpdateAsync(id, dto);
            return Ok(ApiResponse<ChecklistDto>.Ok(result, "Checklist updated"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<ChecklistDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try {
            await _svc.DeleteAsync(id);
            return Ok(ApiResponse.OkNoData("Checklist deleted"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    // ── Checklist Items ───────────────────────────────────────

    [HttpPost("{id:int}/items")]
    public async Task<IActionResult> AddItem(
        int id, [FromBody] CreateChecklistItemDto dto)
    {
        try {
            var result = await _svc.AddItemAsync(id, dto);
            return Ok(ApiResponse<ChecklistItemDto>.Ok(
                result, "Item added"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<ChecklistItemDto>.Fail(ex.Message));
        }
    }

    [HttpPut("items/{itemId:int}")]
    public async Task<IActionResult> UpdateItem(
        int itemId, [FromBody] UpdateChecklistItemDto dto)
    {
        try {
            var result = await _svc.UpdateItemAsync(itemId, dto);
            return Ok(ApiResponse<ChecklistItemDto>.Ok(
                result, "Item updated"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<ChecklistItemDto>.Fail(ex.Message));
        }
    }

    [HttpPut("items/{itemId:int}/toggle")]
    public async Task<IActionResult> ToggleItem(int itemId)
    {
        try {
            var result = await _svc.ToggleItemAsync(itemId);
            var msg    = result.IsCompleted ? "Item completed ✅" : "Item uncompleted ⬜";
            return Ok(ApiResponse<ChecklistItemDto>.Ok(result, msg));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<ChecklistItemDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("items/{itemId:int}")]
    public async Task<IActionResult> DeleteItem(int itemId)
    {
        try {
            await _svc.DeleteItemAsync(itemId);
            return Ok(ApiResponse.OkNoData("Item deleted"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }
}