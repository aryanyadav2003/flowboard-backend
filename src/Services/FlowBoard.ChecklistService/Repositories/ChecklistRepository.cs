using FlowBoard.ChecklistService.Data;
using FlowBoard.ChecklistService.Entities;
using FlowBoard.ChecklistService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.ChecklistService.Repositories;

public class ChecklistRepository : IChecklistRepository
{
    private readonly ChecklistDbContext _db;
    public ChecklistRepository(ChecklistDbContext db) => _db = db;

    // ── Checklist ─────────────────────────────────────────────

    public Task<Checklist?> GetByIdAsync(int checklistId)
        => _db.Checklists.FindAsync(checklistId).AsTask();

    public Task<Checklist?> GetByIdWithItemsAsync(int checklistId)
        => _db.Checklists
              .Include(c => c.Items.OrderBy(i => i.Position))
              .FirstOrDefaultAsync(c => c.ChecklistId == checklistId);

    public Task<List<Checklist>> GetByCardIdAsync(int cardId)
        => _db.Checklists
              .Where(c => c.CardId == cardId)
              .Include(c => c.Items.OrderBy(i => i.Position))
              .OrderBy(c => c.CreatedAt)
              .ToListAsync();

    public Task<bool> ExistsAsync(int checklistId)
        => _db.Checklists.AnyAsync(c => c.ChecklistId == checklistId);

    public async Task<Checklist> CreateAsync(Checklist checklist)
    {
        _db.Checklists.Add(checklist);
        await _db.SaveChangesAsync();
        return checklist;
    }

    public async Task<Checklist> UpdateAsync(Checklist checklist)
    {
        _db.Checklists.Update(checklist);
        await _db.SaveChangesAsync();
        return checklist;
    }

    public async Task DeleteAsync(int checklistId)
    {
        var checklist = await GetByIdAsync(checklistId);
        if (checklist != null)
        {
            _db.Checklists.Remove(checklist);
            await _db.SaveChangesAsync();
        }
    }

    // ── Items ─────────────────────────────────────────────────

    public Task<ChecklistItem?> GetItemByIdAsync(int itemId)
        => _db.ChecklistItems.FindAsync(itemId).AsTask();

    public Task<List<ChecklistItem>> GetItemsByChecklistIdAsync(int checklistId)
        => _db.ChecklistItems
              .Where(i => i.ChecklistId == checklistId)
              .OrderBy(i => i.Position)
              .ToListAsync();

    public async Task<int> GetMaxItemPositionAsync(int checklistId)
    {
        var items = await _db.ChecklistItems
            .Where(i => i.ChecklistId == checklistId)
            .ToListAsync();
        return items.Any() ? items.Max(i => i.Position) : -1;
    }

    public async Task<ChecklistItem> CreateItemAsync(ChecklistItem item)
    {
        _db.ChecklistItems.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<ChecklistItem> UpdateItemAsync(ChecklistItem item)
    {
        _db.ChecklistItems.Update(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task DeleteItemAsync(int itemId)
    {
        var item = await GetItemByIdAsync(itemId);
        if (item != null)
        {
            _db.ChecklistItems.Remove(item);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdateItemPositionsAsync(List<ChecklistItem> items)
    {
        _db.ChecklistItems.UpdateRange(items);
        await _db.SaveChangesAsync();
    }
}