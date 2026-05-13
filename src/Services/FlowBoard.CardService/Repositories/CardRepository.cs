using FlowBoard.CardService.Data;
using FlowBoard.CardService.Entities;
using FlowBoard.CardService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.CardService.Repositories;

public class CardRepository : ICardRepository
{
    private readonly CardDbContext _db;
    public CardRepository(CardDbContext db) => _db = db;

    public Task<Card?> GetByIdAsync(int cardId)
        => _db.Cards.FindAsync(cardId).AsTask();

    public Task<Card?> GetByIdWithAssigneesAsync(int cardId)
        => _db.Cards
              .Include(c => c.Assignees)
              .FirstOrDefaultAsync(c => c.CardId == cardId);

    public Task<List<Card>> GetByListIdAsync(int listId)
        => _db.Cards
              .Where(c => c.ListId == listId && !c.IsArchived)
              .Include(c => c.Assignees)
              .OrderBy(c => c.Position)
              .ToListAsync();

    public Task<List<Card>> GetByBoardIdAsync(int boardId)
        => _db.Cards
              .Where(c => c.BoardId == boardId && !c.IsArchived)
              .Include(c => c.Assignees)
              .OrderBy(c => c.ListId)
              .ThenBy(c => c.Position)
              .ToListAsync();

    public Task<List<Card>> GetOverdueCardsAsync()
        => _db.Cards
              .Where(c => !c.IsArchived &&
                          c.DueDate.HasValue &&
                          c.DueDate.Value < DateTime.UtcNow &&
                          c.Status != "DONE")
              .Include(c => c.Assignees)
              .ToListAsync();

    public Task<bool> ExistsAsync(int cardId)
        => _db.Cards.AnyAsync(c => c.CardId == cardId);

    public async Task<int> GetMaxPositionAsync(int listId)
    {
        var cards = await _db.Cards
            .Where(c => c.ListId == listId && !c.IsArchived)
            .ToListAsync();
        return cards.Any() ? cards.Max(c => c.Position) : -1;
    }

    public async Task<Card> CreateAsync(Card card)
    {
        _db.Cards.Add(card);
        await _db.SaveChangesAsync();
        return card;
    }

    public async Task<Card> UpdateAsync(Card card)
    {
        _db.Cards.Update(card);
        await _db.SaveChangesAsync();
        return card;
    }

    public async Task DeleteAsync(int cardId)
    {
        var card = await GetByIdAsync(cardId);
        if (card != null)
        {
            _db.Cards.Remove(card);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdatePositionsAsync(List<Card> cards)
    {
        _db.Cards.UpdateRange(cards);
        await _db.SaveChangesAsync();
    }

    public Task<CardAssignee?> GetAssigneeAsync(int cardId, int userId)
        => _db.CardAssignees
              .FirstOrDefaultAsync(a => a.CardId == cardId
                                     && a.UserId == userId);

    public Task<List<CardAssignee>> GetAssigneesAsync(int cardId)
        => _db.CardAssignees
              .Where(a => a.CardId == cardId)
              .ToListAsync();

    public Task<bool> IsAssignedAsync(int cardId, int userId)
        => _db.CardAssignees
              .AnyAsync(a => a.CardId == cardId && a.UserId == userId);

    public async Task<CardAssignee> AddAssigneeAsync(CardAssignee assignee)
    {
        _db.CardAssignees.Add(assignee);
        await _db.SaveChangesAsync();
        return assignee;
    }

    public async Task RemoveAssigneeAsync(int cardId, int userId)
    {
        var assignee = await GetAssigneeAsync(cardId, userId);
        if (assignee != null)
        {
            _db.CardAssignees.Remove(assignee);
            await _db.SaveChangesAsync();
        }
    }

    public Task<List<Card>> GetCardsByAssigneeAsync(int userId)
        => _db.Cards
              .Where(c => c.Assignees.Any(a => a.UserId == userId) && !c.IsArchived)
              .Include(c => c.Assignees)
              .OrderBy(c => c.BoardId)
              .ToListAsync();
}