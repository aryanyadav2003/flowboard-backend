using FlowBoard.BoardService.Data;
using FlowBoard.BoardService.Entities;
using FlowBoard.BoardService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.BoardService.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly BoardDbContext _db;
    public BoardRepository(BoardDbContext db) => _db = db;

    public Task<Board?> GetByIdAsync(int id)
        => _db.Boards.FindAsync(id).AsTask();

    public Task<Board?> GetByIdWithMembersAsync(int id)
        => _db.Boards.Include(b => b.Members)
              .FirstOrDefaultAsync(b => b.BoardId == id);

    public Task<List<Board>> GetByWorkspaceIdAsync(int workspaceId)
        => _db.Boards
              .Where(b => b.WorkspaceId == workspaceId && b.IsActive)
              .Include(b => b.Members)
              .ToListAsync();

    public Task<List<Board>> GetByMemberIdAsync(int userId)
        => _db.Boards
              .Where(b => b.IsActive && b.Members.Any(m => m.UserId == userId))
              .Include(b => b.Members)
              .ToListAsync();

    public Task<List<Board>> GetPublicAsync()
        => _db.Boards
              .Where(b => b.Visibility == "PUBLIC" && b.IsActive)
              .Include(b => b.Members)
              .ToListAsync();

    public Task<bool> ExistsAsync(int id)
        => _db.Boards.AnyAsync(b => b.BoardId == id);

    public async Task<Board> CreateAsync(Board b)
    {
        _db.Boards.Add(b);
        await _db.SaveChangesAsync();
        return b;
    }

    public async Task<Board> UpdateAsync(Board b)
    {
        _db.Boards.Update(b);
        await _db.SaveChangesAsync();
        return b;
    }

    public async Task DeleteAsync(int id)
    {
        var b = await GetByIdAsync(id);
        if (b != null) { _db.Boards.Remove(b); await _db.SaveChangesAsync(); }
    }

    public Task<BoardMember?> GetMemberAsync(int boardId, int userId)
        => _db.BoardMembers
              .FirstOrDefaultAsync(m => m.BoardId == boardId && m.UserId == userId);

    public Task<List<BoardMember>> GetMembersAsync(int boardId)
        => _db.BoardMembers.Where(m => m.BoardId == boardId).ToListAsync();

    public Task<bool> IsMemberAsync(int boardId, int userId)
        => _db.BoardMembers.AnyAsync(m => m.BoardId == boardId && m.UserId == userId);

    public async Task<BoardMember> AddMemberAsync(BoardMember m)
    {
        _db.BoardMembers.Add(m);
        await _db.SaveChangesAsync();
        return m;
    }

    public async Task<BoardMember> UpdateMemberAsync(BoardMember m)
    {
        _db.BoardMembers.Update(m);
        await _db.SaveChangesAsync();
        return m;
    }

    public async Task RemoveMemberAsync(int boardId, int userId)
    {
        var m = await GetMemberAsync(boardId, userId);
        if (m != null) { _db.BoardMembers.Remove(m); await _db.SaveChangesAsync(); }
    }
}