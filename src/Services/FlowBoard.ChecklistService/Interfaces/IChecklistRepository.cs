using FlowBoard.ChecklistService.Entities;

namespace FlowBoard.ChecklistService.Interfaces;

public interface IChecklistRepository
{
    // Checklist operations
    Task<Checklist?>       GetByIdAsync(int checklistId);
    Task<Checklist?>       GetByIdWithItemsAsync(int checklistId);
    Task<List<Checklist>>  GetByCardIdAsync(int cardId);
    Task<bool>             ExistsAsync(int checklistId);
    Task<Checklist>        CreateAsync(Checklist checklist);
    Task<Checklist>        UpdateAsync(Checklist checklist);
    Task                   DeleteAsync(int checklistId);

    // Item operations
    Task<ChecklistItem?>       GetItemByIdAsync(int itemId);
    Task<List<ChecklistItem>>  GetItemsByChecklistIdAsync(int checklistId);
    Task<int>                  GetMaxItemPositionAsync(int checklistId);
    Task<ChecklistItem>        CreateItemAsync(ChecklistItem item);
    Task<ChecklistItem>        UpdateItemAsync(ChecklistItem item);
    Task                       DeleteItemAsync(int itemId);
    Task                       UpdateItemPositionsAsync(List<ChecklistItem> items);
}