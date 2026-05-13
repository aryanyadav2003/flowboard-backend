using FlowBoard.ChecklistService.DTOs;

namespace FlowBoard.ChecklistService.Interfaces;

public interface IChecklistService
{
    // Checklist operations
    Task<ChecklistDto>       CreateAsync(CreateChecklistDto dto);
    Task<ChecklistDto>       GetByIdAsync(int checklistId);
    Task<List<ChecklistDto>> GetByCardIdAsync(int cardId);
    Task<ChecklistDto>       UpdateAsync(int checklistId, UpdateChecklistDto dto);
    Task                     DeleteAsync(int checklistId);

    // Item operations
    Task<ChecklistItemDto>   AddItemAsync(int checklistId, CreateChecklistItemDto dto);
    Task<ChecklistItemDto>   UpdateItemAsync(int itemId, UpdateChecklistItemDto dto);
    Task<ChecklistItemDto>   ToggleItemAsync(int itemId);
    Task                     DeleteItemAsync(int itemId);
}