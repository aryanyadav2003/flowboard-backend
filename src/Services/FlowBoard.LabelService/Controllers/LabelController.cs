using FlowBoard.LabelService.DTOs;
using FlowBoard.LabelService.Interfaces;
using FlowBoard.Shared.DTOs;
using FlowBoard.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.LabelService.Controllers;

[ApiController]
[Route("api/labels")]
[Authorize]
public class LabelController : ControllerBase
{
    private readonly ILabelService _svc;
    public LabelController(ILabelService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLabelDto dto)
    {
        try {
            var result = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById),
                new { id = result.LabelId },
                ApiResponse<LabelDto>.Ok(result, "Label created"));
        }
        catch (Exception ex) {
            return BadRequest(ApiResponse<LabelDto>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try {
            var result = await _svc.GetByIdAsync(id);
            return Ok(ApiResponse<LabelDto>.Ok(result));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<LabelDto>.Fail(ex.Message));
        }
    }

    [HttpGet("board/{boardId:int}")]
    public async Task<IActionResult> GetByBoard(int boardId)
    {
        var result = await _svc.GetByBoardIdAsync(boardId);
        return Ok(ApiResponse<List<LabelDto>>.Ok(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, [FromBody] UpdateLabelDto dto)
    {
        try {
            var result = await _svc.UpdateAsync(id, dto);
            return Ok(ApiResponse<LabelDto>.Ok(result, "Label updated"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<LabelDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try {
            await _svc.DeleteAsync(id);
            return Ok(ApiResponse.OkNoData("Label deleted"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    // ── Card Assignment ───────────────────────────────────────

    [HttpPost("{id:int}/assign")]
    public async Task<IActionResult> AssignToCard(
        int id, [FromBody] AssignLabelDto dto)
    {
        try {
            var result = await _svc.AssignToCardAsync(id, dto);
            return Ok(ApiResponse<CardLabelDto>.Ok(
                result, "Label assigned to card"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<CardLabelDto>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex) {
            return Conflict(ApiResponse<CardLabelDto>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id:int}/unassign/{cardId:int}")]
    public async Task<IActionResult> UnassignFromCard(int id, int cardId)
    {
        try {
            await _svc.UnassignFromCardAsync(id, cardId);
            return Ok(ApiResponse.OkNoData("Label removed from card"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
    }

    [HttpGet("card/{cardId:int}")]
    public async Task<IActionResult> GetCardLabels(int cardId)
    {
        var result = await _svc.GetCardLabelsAsync(cardId);
        return Ok(ApiResponse<List<CardLabelDto>>.Ok(result));
    }
}