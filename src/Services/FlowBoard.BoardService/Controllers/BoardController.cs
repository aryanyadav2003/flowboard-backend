using FlowBoard.BoardService.DTOs;
using FlowBoard.BoardService.Interfaces;
using FlowBoard.Shared.DTOs;
using FlowBoard.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.BoardService.Controllers;

[ApiController]
[Route("api/boards")]
[Authorize]
public class BoardController : ControllerBase
{
    private readonly IBoardService _svc;
    public BoardController(IBoardService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBoardDto dto)
    {
        try {
            var result = await _svc.CreateAsync(User.GetUserId(), dto);
            return Ok(ApiResponse<BoardDto>.Ok(result, "Board created"));
        }
        catch (Exception ex) { return BadRequest(ApiResponse<BoardDto>.Fail(ex.Message)); }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try {
            var result = await _svc.GetByIdAsync(id, User.GetUserId());
            return Ok(ApiResponse<BoardDto>.Ok(result));
        }
        catch (KeyNotFoundException ex)        { return NotFound(ApiResponse<BoardDto>.Fail(ex.Message)); }
        catch (UnauthorizedAccessException)    { return Forbid(); }
    }

    [HttpGet("workspace/{workspaceId:int}")]
    public async Task<IActionResult> GetByWorkspace(int workspaceId)
    {
        var result = await _svc.GetByWorkspaceAsync(workspaceId, User.GetUserId());
        return Ok(ApiResponse<List<BoardDto>>.Ok(result));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBoards()
    {
        var result = await _svc.GetMyBoardsAsync(User.GetUserId());
        return Ok(ApiResponse<List<BoardDto>>.Ok(result));
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublic()
    {
        var result = await _svc.GetPublicBoardsAsync();
        return Ok(ApiResponse<List<BoardDto>>.Ok(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBoardDto dto)
    {
        try {
            var result = await _svc.UpdateAsync(id, User.GetUserId(), dto);
            return Ok(ApiResponse<BoardDto>.Ok(result, "Board updated"));
        }
        catch (KeyNotFoundException ex)        { return NotFound(ApiResponse<BoardDto>.Fail(ex.Message)); }
        catch (UnauthorizedAccessException)    { return Forbid(); }
    }

    [HttpPut("{id:int}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        try {
            await _svc.ArchiveBoardAsync(id, User.GetUserId());
            return Ok(ApiResponse.OkNoData("Board archived"));
        }
        catch (KeyNotFoundException ex)        { return NotFound(ApiResponse.Fail(ex.Message)); }
        catch (UnauthorizedAccessException)    { return Forbid(); }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try {
            await _svc.DeleteAsync(id, User.GetUserId());
            return Ok(ApiResponse.OkNoData("Board deleted"));
        }
        catch (KeyNotFoundException ex)        { return NotFound(ApiResponse.Fail(ex.Message)); }
        catch (UnauthorizedAccessException)    { return Forbid(); }
    }

    // ── Members ──────────────────────────────────────────────

    [HttpPost("{id:int}/members")]
    public async Task<IActionResult> AddMember(int id, [FromBody] AddBoardMemberDto dto)
    {
        try {
            var result = await _svc.AddMemberAsync(id, User.GetUserId(), dto);
            return Ok(ApiResponse<BoardMemberDto>.Ok(result, "Member added"));
        }
        catch (UnauthorizedAccessException)  { return Forbid(); }
        catch (InvalidOperationException ex) { return Conflict(ApiResponse<BoardMemberDto>.Fail(ex.Message)); }
    }

    [HttpGet("{id:int}/members")]
    public async Task<IActionResult> GetMembers(int id)
    {
        try {
            var result = await _svc.GetMembersAsync(id, User.GetUserId());
            return Ok(ApiResponse<List<BoardMemberDto>>.Ok(result));
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPut("{id:int}/members/{userId:int}/role")]
    public async Task<IActionResult> UpdateRole(int id, int userId, [FromBody] UpdateBoardMemberRoleDto dto)
    {
        try {
            var result = await _svc.UpdateMemberRoleAsync(id, userId, User.GetUserId(), dto);
            return Ok(ApiResponse<BoardMemberDto>.Ok(result, "Role updated"));
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException ex)     { return NotFound(ApiResponse<BoardMemberDto>.Fail(ex.Message)); }
    }

    [HttpDelete("{id:int}/members/{userId:int}")]
    public async Task<IActionResult> RemoveMember(int id, int userId)
    {
        try {
            await _svc.RemoveMemberAsync(id, userId, User.GetUserId());
            return Ok(ApiResponse.OkNoData("Member removed"));
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException ex)     { return NotFound(ApiResponse.Fail(ex.Message)); }
    }

    [HttpDelete("{id:int}/leave")]
    public async Task<IActionResult> Leave(int id)
    {
        try {
            await _svc.LeaveAsync(id, User.GetUserId());
            return Ok(ApiResponse.OkNoData("Left board"));
        }
        catch (InvalidOperationException ex) { return BadRequest(ApiResponse.Fail(ex.Message)); }
        catch (KeyNotFoundException ex)      { return NotFound(ApiResponse.Fail(ex.Message)); }
    }
}