using FlowBoard.CommentService.DTOs;
using FlowBoard.CommentService.Interfaces;
using FlowBoard.Shared.DTOs;
using FlowBoard.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.CommentService.Controllers;

[ApiController]
[Route("api/comments")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ICommentService _svc;
    public CommentController(ICommentService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
    {
        try {
            var result = await _svc.CreateAsync(dto, User.GetUserId());
            return CreatedAtAction(nameof(GetById),
                new { id = result.CommentId },
                ApiResponse<CommentDto>.Ok(result, "Comment added"));
        }
        catch (Exception ex) {
            return BadRequest(ApiResponse<CommentDto>.Fail(ex.Message));
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try {
            var result = await _svc.GetByIdAsync(id);
            return Ok(ApiResponse<CommentDto>.Ok(result));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<CommentDto>.Fail(ex.Message));
        }
    }

    [HttpGet("card/{cardId:int}")]
    public async Task<IActionResult> GetByCard(int cardId)
    {
        var result = await _svc.GetByCardIdAsync(cardId);
        return Ok(ApiResponse<List<CommentDto>>.Ok(result));
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var result = await _svc.GetByUserIdAsync(userId);
        return Ok(ApiResponse<List<CommentDto>>.Ok(result));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyComments()
    {
        var result = await _svc.GetByUserIdAsync(User.GetUserId());
        return Ok(ApiResponse<List<CommentDto>>.Ok(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, [FromBody] UpdateCommentDto dto)
    {
        try {
            var result = await _svc.UpdateAsync(id, dto, User.GetUserId());
            return Ok(ApiResponse<CommentDto>.Ok(result, "Comment updated"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse<CommentDto>.Fail(ex.Message));
        }
        catch (UnauthorizedAccessException ex) {
            return Forbid();
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try {
            await _svc.DeleteAsync(id, User.GetUserId());
            return Ok(ApiResponse.OkNoData("Comment deleted"));
        }
        catch (KeyNotFoundException ex) {
            return NotFound(ApiResponse.Fail(ex.Message));
        }
        catch (UnauthorizedAccessException) {
            return Forbid();
        }
    }
}