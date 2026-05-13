using FlowBoard.CardService.DTOs;

namespace FlowBoard.CardService.Interfaces;

public interface ICardService
{
    Task<CardDto>            CreateAsync(CreateCardDto dto, int requesterId);
    Task<CardDto>            GetByIdAsync(int cardId);
    Task<List<CardDto>>      GetByListIdAsync(int listId);
    Task<List<CardDto>>      GetByBoardIdAsync(int boardId);
    Task<List<CardDto>>      GetOverdueAsync();
    Task<CardDto>            UpdateAsync(int cardId, UpdateCardDto dto, int requesterId);
    Task<CardDto>            MoveAsync(int cardId, MoveCardDto dto, int requesterId);
    Task                     ArchiveAsync(int cardId, int requesterId);
    Task                     DeleteAsync(int cardId, int requesterId);
    Task<CardAssigneeDto>    AssignUserAsync(int cardId, AssignUserDto dto, int requesterId);
    Task                     UnassignUserAsync(int cardId, int userId, int requesterId);
    Task<List<CardAssigneeDto>> GetAssigneesAsync(int cardId);
    Task<List<CardDto>>      GetMyTasksAsync(int userId);
}