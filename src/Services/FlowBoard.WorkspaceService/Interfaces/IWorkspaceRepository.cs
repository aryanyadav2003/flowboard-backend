using FlowBoard.WorkspaceService.Entities;

namespace FlowBoard.WorkspaceService.Interfaces;

public interface IWorkspaceRepository
{
    Task<Workspace?>            GetByIdAsync(int workspaceId);
    Task<Workspace?>            GetByIdWithMembersAsync(int workspaceId);
    Task<List<Workspace>>       GetByOwnerIdAsync(int ownerId);
    Task<List<Workspace>>       GetByMemberIdAsync(int userId);
    Task<List<Workspace>>       GetPublicAsync();
    Task<bool>                  ExistsAsync(int workspaceId);
    Task<Workspace>             CreateAsync(Workspace workspace);
    Task<Workspace>             UpdateAsync(Workspace workspace);
    Task                        DeleteAsync(int workspaceId);

    Task<WorkspaceMember?>      GetMemberAsync(int workspaceId, int userId);
    Task<List<WorkspaceMember>> GetMembersAsync(int workspaceId);
    Task<bool>                  IsMemberAsync(int workspaceId, int userId);
    Task<WorkspaceMember>       AddMemberAsync(WorkspaceMember member);
    Task<WorkspaceMember>       UpdateMemberAsync(WorkspaceMember member);
    Task                        RemoveMemberAsync(int workspaceId, int userId);
}