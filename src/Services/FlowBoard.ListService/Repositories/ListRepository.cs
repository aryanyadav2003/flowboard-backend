using FlowBoard.ListService.Data;
using FlowBoard.ListService.Entities;
using FlowBoard.ListService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.ListService.Repositories;

public class ListRepository : IListRepository
{
    private readonly ListDbContext _db;
    public ListRepository(ListDbContext db) => _db = db;

    public Task<TaskList?> GetByIdAsync(int listId)
        => _db.TaskLists.FindAsync(listId).AsTask();

    public Task<List<TaskList>> GetByBoardIdAsync(int boardId)
        => _db.TaskLists
              .Where(l => l.BoardId == boardId && !l.IsArchived)
              .OrderBy(l => l.Position)
              .ToListAsync();

    public Task<bool> ExistsAsync(int listId)
        => _db.TaskLists.AnyAsync(l => l.ListId == listId);

    public async Task<int> GetMaxPositionAsync(int boardId)
    {
        var lists = await _db.TaskLists
            .Where(l => l.BoardId == boardId && !l.IsArchived)
            .ToListAsync();

        return lists.Any() ? lists.Max(l => l.Position) : -1;
    }

    public async Task<TaskList> CreateAsync(TaskList list)
    {
        _db.TaskLists.Add(list);
        await _db.SaveChangesAsync();
        return list;
    }

    public async Task<TaskList> UpdateAsync(TaskList list)
    {
        _db.TaskLists.Update(list);
        await _db.SaveChangesAsync();
        return list;
    }

    public async Task DeleteAsync(int listId)
    {
        var list = await GetByIdAsync(listId);
        if (list != null)
        {
            _db.TaskLists.Remove(list);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdatePositionsAsync(List<TaskList> lists)
    {
        _db.TaskLists.UpdateRange(lists);
        await _db.SaveChangesAsync();
    }
}