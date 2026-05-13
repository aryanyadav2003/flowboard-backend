using FlowBoard.WorkspaceService.Data;
using FlowBoard.WorkspaceService.Entities;
using FlowBoard.WorkspaceService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.WorkspaceService.Repositories;

public class WorkspaceRepository : IWorkspaceRepository
{
    private readonly WorkspaceDbContext _db;
    public WorkspaceRepository(WorkspaceDbContext db) => _db = db;

    public Task<Workspace?> GetByIdAsync(int id)
        => _db.Workspaces.FindAsync(id).AsTask();

    public Task<Workspace?> GetByIdWithMembersAsync(int id)
        => _db.Workspaces.Include(w => w.Members)
              .FirstOrDefaultAsync(w => w.WorkspaceId == id);

    public Task<List<Workspace>> GetByOwnerIdAsync(int ownerId)
        => _db.Workspaces.Where(w => w.OwnerId == ownerId && w.IsActive)
              .Include(w => w.Members).ToListAsync();

    public Task<List<Workspace>> GetByMemberIdAsync(int userId)
        => _db.Workspaces
              .Where(w => w.IsActive && w.Members.Any(m => m.UserId == userId))
              .Include(w => w.Members).ToListAsync();

    public Task<List<Workspace>> GetPublicAsync()
        => _db.Workspaces
              .Where(w => w.Visibility == "PUBLIC" && w.IsActive)
              .Include(w => w.Members).ToListAsync();

    public Task<bool> ExistsAsync(int id)
        => _db.Workspaces.AnyAsync(w => w.WorkspaceId == id);

    public async Task<Workspace> CreateAsync(Workspace w)
    {
        _db.Workspaces.Add(w);
        await _db.SaveChangesAsync();
        return w;
    }

    public async Task<Workspace> UpdateAsync(Workspace w)
    {
        _db.Workspaces.Update(w);
        await _db.SaveChangesAsync();
        return w;
    }

    public async Task DeleteAsync(int id)
    {
        var w = await GetByIdAsync(id);
        if (w != null) { _db.Workspaces.Remove(w); await _db.SaveChangesAsync(); }
    }

    public Task<WorkspaceMember?> GetMemberAsync(int workspaceId, int userId)
        => _db.WorkspaceMembers
              .FirstOrDefaultAsync(m => m.WorkspaceId == workspaceId && m.UserId == userId);

    public Task<List<WorkspaceMember>> GetMembersAsync(int workspaceId)
        => _db.WorkspaceMembers.Where(m => m.WorkspaceId == workspaceId).ToListAsync();

    public Task<bool> IsMemberAsync(int workspaceId, int userId)
        => _db.WorkspaceMembers.AnyAsync(m => m.WorkspaceId == workspaceId && m.UserId == userId);

    public async Task<WorkspaceMember> AddMemberAsync(WorkspaceMember m)
    {
        _db.WorkspaceMembers.Add(m);
        await _db.SaveChangesAsync();
        return m;
    }

    public async Task<WorkspaceMember> UpdateMemberAsync(WorkspaceMember m)
    {
        _db.WorkspaceMembers.Update(m);
        await _db.SaveChangesAsync();
        return m;
    }

    public async Task RemoveMemberAsync(int workspaceId, int userId)
    {
        var m = await GetMemberAsync(workspaceId, userId);
        if (m != null) { _db.WorkspaceMembers.Remove(m); await _db.SaveChangesAsync(); }
    }
}