    using FlowBoard.WorkspaceService.DTOs;

namespace FlowBoard.WorkspaceService.Interfaces;

public interface IWorkspaceService
{
    Task<WorkspaceDto>            CreateAsync(int ownerId, CreateWorkspaceDto dto);
    Task<WorkspaceDto>            GetByIdAsync(int workspaceId, int requesterId);
    Task<List<WorkspaceDto>>      GetMyWorkspacesAsync(int userId);
    Task<List<WorkspaceDto>>      GetPublicWorkspacesAsync();
    Task<WorkspaceDto>            UpdateAsync(int workspaceId, int requesterId, UpdateWorkspaceDto dto);
    Task                          DeleteAsync(int workspaceId, int requesterId);

    Task<WorkspaceMemberDto>      AddMemberAsync(int workspaceId, int requesterId, AddMemberDto dto);
    Task<List<WorkspaceMemberDto>> GetMembersAsync(int workspaceId, int requesterId);
    Task<WorkspaceMemberDto>      UpdateMemberRoleAsync(int workspaceId, int targetUserId, int requesterId, UpdateMemberRoleDto dto);
    Task                          RemoveMemberAsync(int workspaceId, int targetUserId, int requesterId);
    Task                          LeaveWorkspaceAsync(int workspaceId, int userId);
}