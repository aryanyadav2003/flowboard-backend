using FlowBoard.CardService.Entities;

namespace FlowBoard.CardService.Interfaces;

public interface ICardRepository
{
    Task<Card?>            GetByIdAsync(int cardId);
    Task<Card?>            GetByIdWithAssigneesAsync(int cardId);
    Task<List<Card>>       GetByListIdAsync(int listId);
    Task<List<Card>>       GetByBoardIdAsync(int boardId);
    Task<List<Card>>       GetOverdueCardsAsync();
    Task<bool>             ExistsAsync(int cardId);
    Task<int>              GetMaxPositionAsync(int listId);
    Task<Card>             CreateAsync(Card card);
    Task<Card>             UpdateAsync(Card card);
    Task                   DeleteAsync(int cardId);
    Task                   UpdatePositionsAsync(List<Card> cards);

    Task<CardAssignee?>    GetAssigneeAsync(int cardId, int userId);
    Task<List<CardAssignee>> GetAssigneesAsync(int cardId);
    Task<bool>             IsAssignedAsync(int cardId, int userId);
    Task<CardAssignee>     AddAssigneeAsync(CardAssignee assignee);
    Task                   RemoveAssigneeAsync(int cardId, int userId);
    Task<List<Card>>       GetCardsByAssigneeAsync(int userId);
}