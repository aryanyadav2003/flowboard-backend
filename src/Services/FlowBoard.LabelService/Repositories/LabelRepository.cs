using FlowBoard.LabelService.Data;
using FlowBoard.LabelService.Entities;
using FlowBoard.LabelService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.LabelService.Repositories;

public class LabelRepository : ILabelRepository
{
    private readonly LabelDbContext _db;
    public LabelRepository(LabelDbContext db) => _db = db;

    public Task<Label?> GetByIdAsync(int labelId)
        => _db.Labels.FindAsync(labelId).AsTask();

    public Task<Label?> GetByIdWithCardLabelsAsync(int labelId)
        => _db.Labels
              .Include(l => l.CardLabels)
              .FirstOrDefaultAsync(l => l.LabelId == labelId);

    public Task<List<Label>> GetByBoardIdAsync(int boardId)
        => _db.Labels
              .Where(l => l.BoardId == boardId)
              .OrderBy(l => l.Name)
              .ToListAsync();

    public Task<bool> ExistsAsync(int labelId)
        => _db.Labels.AnyAsync(l => l.LabelId == labelId);

    public async Task<Label> CreateAsync(Label label)
    {
        _db.Labels.Add(label);
        await _db.SaveChangesAsync();
        return label;
    }

    public async Task<Label> UpdateAsync(Label label)
    {
        _db.Labels.Update(label);
        await _db.SaveChangesAsync();
        return label;
    }

    public async Task DeleteAsync(int labelId)
    {
        var label = await GetByIdAsync(labelId);
        if (label != null)
        {
            _db.Labels.Remove(label);
            await _db.SaveChangesAsync();
        }
    }

    public Task<CardLabel?> GetCardLabelAsync(int cardId, int labelId)
        => _db.CardLabels
              .Include(cl => cl.Label)
              .FirstOrDefaultAsync(cl =>
                  cl.CardId == cardId && cl.LabelId == labelId);

    public Task<List<CardLabel>> GetCardLabelsAsync(int cardId)
        => _db.CardLabels
              .Where(cl => cl.CardId == cardId)
              .Include(cl => cl.Label)
              .ToListAsync();

    public Task<bool> IsAssignedAsync(int cardId, int labelId)
        => _db.CardLabels.AnyAsync(cl =>
               cl.CardId == cardId && cl.LabelId == labelId);

    public async Task<CardLabel> AssignAsync(CardLabel cardLabel)
    {
        _db.CardLabels.Add(cardLabel);
        await _db.SaveChangesAsync();
        return cardLabel;
    }

    public async Task UnassignAsync(int cardId, int labelId)
    {
        var cardLabel = await GetCardLabelAsync(cardId, labelId);
        if (cardLabel != null)
        {
            _db.CardLabels.Remove(cardLabel);
            await _db.SaveChangesAsync();
        }
    }
}