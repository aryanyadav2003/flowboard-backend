using FlowBoard.Shared.DTOs;
using FlowBoard.Shared.Extensions;
using FlowBoard.WorkspaceService.DTOs;
using FlowBoard.WorkspaceService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.WorkspaceService.Controllers;

[ApiController]
[Route("api/workspaces")]
[Authorize]
public class WorkspaceController : ControllerBase
{
    private readonly IWorkspaceService _svc;
    public WorkspaceController(IWorkspaceService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkspaceDto dto)
    {
        try {
            var result = await _svc.CreateAsync(User.GetUserId(), dto);
            return CreatedAtAction(nameof(GetById), new { id = result.WorkspaceId },
                ApiResponse<WorkspaceDto>.Ok(result, "Workspace created"));
        }
        catch (Exception ex) { return BadRequest(ApiResponse<WorkspaceDto>.Fail(ex.Message)); }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try {
            var result = await _svc.GetByIdAsync(id, User.GetUserId());
            return Ok(ApiResponse<WorkspaceDto>.Ok(result));
        }
        catch (KeyNotFoundException ex)         { return NotFound(ApiResponse<WorkspaceDto>.Fail(ex.Message)); }
        catch (UnauthorizedAccessException ex)  { return Forbid(); }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyWorkspaces()
    {
        var result = await _svc.GetMyWorkspacesAsync(User.GetUserId());
        return Ok(ApiResponse<List<WorkspaceDto>>.Ok(result));
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublic()
    {
        var result = await _svc.GetPublicWorkspacesAsync();
        return Ok(ApiResponse<List<WorkspaceDto>>.Ok(result));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkspaceDto dto)
    {
        try {
            var result = await _svc.UpdateAsync(id, User.GetUserId(), dto);
            return Ok(ApiResponse<WorkspaceDto>.Ok(result, "Workspace updated"));
        }
        catch (KeyNotFoundException ex)        { return NotFound(ApiResponse<WorkspaceDto>.Fail(ex.Message)); }
        catch (UnauthorizedAccessException ex) { return Forbid(); }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try {
            await _svc.DeleteAsync(id, User.GetUserId());
            return Ok(ApiResponse.OkNoData("Workspace deleted"));
        }
        catch (KeyNotFoundException ex)        { return NotFound(ApiResponse.Fail(ex.Message)); }
        catch (UnauthorizedAccessException ex) { return Forbid(); }
    }

    // ── Members ──────────────────────────────────────────────

    [HttpPost("{id:int}/members")]
    public async Task<IActionResult> AddMember(int id, [FromBody] AddMemberDto dto)
    {
        try {
            var result = await _svc.AddMemberAsync(id, User.GetUserId(), dto);
            return Ok(ApiResponse<WorkspaceMemberDto>.Ok(result, "Member added"));
        }
        catch (UnauthorizedAccessException ex) { return Forbid(); }
        catch (InvalidOperationException ex)   { return Conflict(ApiResponse<WorkspaceMemberDto>.Fail(ex.Message)); }
    }

    [HttpGet("{id:int}/members")]
    public async Task<IActionResult> GetMembers(int id)
    {
        try {
            var result = await _svc.GetMembersAsync(id, User.GetUserId());
            return Ok(ApiResponse<List<WorkspaceMemberDto>>.Ok(result));
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPut("{id:int}/members/{userId:int}/role")]
    public async Task<IActionResult> UpdateRole(int id, int userId, [FromBody] UpdateMemberRoleDto dto)
    {
        try {
            var result = await _svc.UpdateMemberRoleAsync(id, userId, User.GetUserId(), dto);
            return Ok(ApiResponse<WorkspaceMemberDto>.Ok(result, "Role updated"));
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (KeyNotFoundException ex)     { return NotFound(ApiResponse<WorkspaceMemberDto>.Fail(ex.Message)); }
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
            await _svc.LeaveWorkspaceAsync(id, User.GetUserId());
            return Ok(ApiResponse.OkNoData("Left workspace"));
        }
        catch (InvalidOperationException ex) { return BadRequest(ApiResponse.Fail(ex.Message)); }
        catch (KeyNotFoundException ex)      { return NotFound(ApiResponse.Fail(ex.Message)); }
    }
}