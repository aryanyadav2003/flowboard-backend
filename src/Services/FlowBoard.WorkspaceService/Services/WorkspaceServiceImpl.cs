using AutoMapper;
using FlowBoard.WorkspaceService.DTOs;
using FlowBoard.WorkspaceService.Entities;
using FlowBoard.WorkspaceService.Interfaces;

namespace FlowBoard.WorkspaceService.Services;

public class WorkspaceServiceImpl : IWorkspaceService
{
    private readonly IWorkspaceRepository _repo;
    private readonly IMapper              _mapper;

    public WorkspaceServiceImpl(IWorkspaceRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<WorkspaceDto> CreateAsync(int ownerId, CreateWorkspaceDto dto)
    {
        var workspace = new Workspace
        {
            Name        = dto.Name,
            Description = dto.Description,
            LogoUrl     = dto.LogoUrl,
            Visibility  = dto.Visibility,
            OwnerId     = ownerId
        };

        await _repo.CreateAsync(workspace);

        // Auto-add owner as OWNER member
        await _repo.AddMemberAsync(new WorkspaceMember
        {
            WorkspaceId = workspace.WorkspaceId,
            UserId      = ownerId,
            Role        = "OWNER"
        });

        var created = await _repo.GetByIdWithMembersAsync(workspace.WorkspaceId);
        return MapToDto(created!);
    }

    public async Task<WorkspaceDto> GetByIdAsync(int workspaceId, int requesterId)
    {
        var workspace = await _repo.GetByIdWithMembersAsync(workspaceId)
            ?? throw new KeyNotFoundException("Workspace not found.");

        if (workspace.Visibility == "PRIVATE" &&
            !await _repo.IsMemberAsync(workspaceId, requesterId))
            throw new UnauthorizedAccessException("Access denied to private workspace.");

        return MapToDto(workspace);
    }

    public async Task<List<WorkspaceDto>> GetMyWorkspacesAsync(int userId)
    {
        var workspaces = await _repo.GetByMemberIdAsync(userId);
        return workspaces.Select(MapToDto).ToList();
    }

    public async Task<List<WorkspaceDto>> GetPublicWorkspacesAsync()
    {
        var workspaces = await _repo.GetPublicAsync();
        return workspaces.Select(MapToDto).ToList();
    }

    public async Task<WorkspaceDto> UpdateAsync(int workspaceId, int requesterId, UpdateWorkspaceDto dto)
    {
        var workspace = await _repo.GetByIdWithMembersAsync(workspaceId)
            ?? throw new KeyNotFoundException("Workspace not found.");

        await EnsureAdminOrOwner(workspaceId, requesterId);

        if (dto.Name        != null) workspace.Name        = dto.Name;
        if (dto.Description != null) workspace.Description = dto.Description;
        if (dto.LogoUrl     != null) workspace.LogoUrl     = dto.LogoUrl;
        if (dto.Visibility  != null) workspace.Visibility  = dto.Visibility;
        workspace.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(workspace);
        return MapToDto(workspace);
    }

    public async Task DeleteAsync(int workspaceId, int requesterId)
    {
        var workspace = await _repo.GetByIdAsync(workspaceId)
            ?? throw new KeyNotFoundException("Workspace not found.");

        if (workspace.OwnerId != requesterId)
            throw new UnauthorizedAccessException("Only the owner can delete this workspace.");

        workspace.IsActive = false;
        await _repo.UpdateAsync(workspace);
    }

    public async Task<WorkspaceMemberDto> AddMemberAsync(int workspaceId, int requesterId, AddMemberDto dto)
    {
        await EnsureAdminOrOwner(workspaceId, requesterId);

        if (await _repo.IsMemberAsync(workspaceId, dto.UserId))
            throw new InvalidOperationException("User is already a member.");

        var member = new WorkspaceMember
        {
            WorkspaceId = workspaceId,
            UserId      = dto.UserId,
            Role        = dto.Role
        };

        await _repo.AddMemberAsync(member);
        return _mapper.Map<WorkspaceMemberDto>(member);
    }

    public async Task<List<WorkspaceMemberDto>> GetMembersAsync(int workspaceId, int requesterId)
    {
        if (!await _repo.IsMemberAsync(workspaceId, requesterId))
            throw new UnauthorizedAccessException("Access denied.");

        var members = await _repo.GetMembersAsync(workspaceId);
        return _mapper.Map<List<WorkspaceMemberDto>>(members);
    }

    public async Task<WorkspaceMemberDto> UpdateMemberRoleAsync(int workspaceId, int targetUserId, int requesterId, UpdateMemberRoleDto dto)
    {
        await EnsureAdminOrOwner(workspaceId, requesterId);

        var member = await _repo.GetMemberAsync(workspaceId, targetUserId)
            ?? throw new KeyNotFoundException("Member not found.");

        member.Role = dto.Role;
        await _repo.UpdateMemberAsync(member);
        return _mapper.Map<WorkspaceMemberDto>(member);
    }

    public async Task RemoveMemberAsync(int workspaceId, int targetUserId, int requesterId)
    {
        await EnsureAdminOrOwner(workspaceId, requesterId);

        if (!await _repo.IsMemberAsync(workspaceId, targetUserId))
            throw new KeyNotFoundException("Member not found.");

        await _repo.RemoveMemberAsync(workspaceId, targetUserId);
    }

    public async Task LeaveWorkspaceAsync(int workspaceId, int userId)
    {
        var workspace = await _repo.GetByIdAsync(workspaceId)
            ?? throw new KeyNotFoundException("Workspace not found.");

        if (workspace.OwnerId == userId)
            throw new InvalidOperationException("Owner cannot leave. Transfer ownership first.");

        await _repo.RemoveMemberAsync(workspaceId, userId);
    }

    // ── Helpers ──────────────────────────────────────────────
    private async Task EnsureAdminOrOwner(int workspaceId, int userId)
    {
        var member = await _repo.GetMemberAsync(workspaceId, userId)
            ?? throw new UnauthorizedAccessException("You are not a member of this workspace.");

        if (member.Role != "OWNER" && member.Role != "ADMIN")
            throw new UnauthorizedAccessException("Only admins or owners can perform this action.");
    }

    private static WorkspaceDto MapToDto(Workspace w) => new()
    {
        WorkspaceId = w.WorkspaceId,
        Name        = w.Name,
        Description = w.Description,
        LogoUrl     = w.LogoUrl,
        Visibility  = w.Visibility,
        OwnerId     = w.OwnerId,
        IsActive    = w.IsActive,
        MemberCount = w.Members.Count,
        CreatedAt   = w.CreatedAt,
        UpdatedAt   = w.UpdatedAt
    };
}