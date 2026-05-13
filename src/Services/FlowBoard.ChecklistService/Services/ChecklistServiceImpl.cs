using AutoMapper;
using FlowBoard.ChecklistService.DTOs;
using FlowBoard.ChecklistService.Entities;
using FlowBoard.ChecklistService.Interfaces;

namespace FlowBoard.ChecklistService.Services;

public class ChecklistServiceImpl : IChecklistService
{
    private readonly IChecklistRepository _repo;
    private readonly IMapper              _mapper;

    public ChecklistServiceImpl(IChecklistRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<ChecklistDto> CreateAsync(CreateChecklistDto dto)
    {
        var checklist = new Checklist
        {
            CardId = dto.CardId,
            Title  = dto.Title
        };

        await _repo.CreateAsync(checklist);

        // Fetch with items (empty for now)
        var created = await _repo.GetByIdWithItemsAsync(checklist.ChecklistId);
        return MapToDto(created!);
    }

    public async Task<ChecklistDto> GetByIdAsync(int checklistId)
    {
        var checklist = await _repo.GetByIdWithItemsAsync(checklistId)
            ?? throw new KeyNotFoundException("Checklist not found.");

        return MapToDto(checklist);
    }

    public async Task<List<ChecklistDto>> GetByCardIdAsync(int cardId)
    {
        var checklists = await _repo.GetByCardIdAsync(cardId);
        return checklists.Select(MapToDto).ToList();
    }

    public async Task<ChecklistDto> UpdateAsync(
        int checklistId, UpdateChecklistDto dto)
    {
        var checklist = await _repo.GetByIdWithItemsAsync(checklistId)
            ?? throw new KeyNotFoundException("Checklist not found.");

        checklist.Title     = dto.Title;
        checklist.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(checklist);
        return MapToDto(checklist);
    }

    public async Task DeleteAsync(int checklistId)
    {
        if (!await _repo.ExistsAsync(checklistId))
            throw new KeyNotFoundException("Checklist not found.");

        // Cascade delete removes all items too
        await _repo.DeleteAsync(checklistId);
    }

    public async Task<ChecklistItemDto> AddItemAsync(
        int checklistId, CreateChecklistItemDto dto)
    {
        if (!await _repo.ExistsAsync(checklistId))
            throw new KeyNotFoundException("Checklist not found.");

        // Place item at end of the checklist
        var maxPosition = await _repo.GetMaxItemPositionAsync(checklistId);

        var item = new ChecklistItem
        {
            ChecklistId = checklistId,
            Text        = dto.Text,
            IsCompleted = false,
            Position    = maxPosition + 1
        };

        await _repo.CreateItemAsync(item);
        return _mapper.Map<ChecklistItemDto>(item);
    }

    public async Task<ChecklistItemDto> UpdateItemAsync(
        int itemId, UpdateChecklistItemDto dto)
    {
        var item = await _repo.GetItemByIdAsync(itemId)
            ?? throw new KeyNotFoundException("Item not found.");

        if (dto.Text != null) item.Text = dto.Text;
        item.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateItemAsync(item);
        return _mapper.Map<ChecklistItemDto>(item);
    }

    public async Task<ChecklistItemDto> ToggleItemAsync(int itemId)
    {
        var item = await _repo.GetItemByIdAsync(itemId)
            ?? throw new KeyNotFoundException("Item not found.");

        // Flip completed status
        item.IsCompleted = !item.IsCompleted;
        item.UpdatedAt   = DateTime.UtcNow;

        await _repo.UpdateItemAsync(item);
        return _mapper.Map<ChecklistItemDto>(item);
    }

    public async Task DeleteItemAsync(int itemId)
    {
        var item = await _repo.GetItemByIdAsync(itemId)
            ?? throw new KeyNotFoundException("Item not found.");

        var checklistId = item.ChecklistId;
        await _repo.DeleteItemAsync(itemId);

        // Re-number remaining items to fill gap
        var remaining = await _repo.GetItemsByChecklistIdAsync(checklistId);
        for (int i = 0; i < remaining.Count; i++)
            remaining[i].Position = i;

        await _repo.UpdateItemPositionsAsync(remaining);
    }

    // ── Helper ───────────────────────────────────────────────
    private static ChecklistDto MapToDto(Checklist c)
    {
        var totalItems     = c.Items.Count;
        var completedItems = c.Items.Count(i => i.IsCompleted);

        // Progress percentage — 0 if no items
        var progress = totalItems == 0
            ? 0
            : (int)Math.Round((double)completedItems / totalItems * 100);

        return new ChecklistDto
        {
            ChecklistId    = c.ChecklistId,
            CardId         = c.CardId,
            Title          = c.Title,
            TotalItems     = totalItems,
            CompletedItems = completedItems,
            Progress       = progress,   // e.g. 75 means 75% done
            CreatedAt      = c.CreatedAt,
            UpdatedAt      = c.UpdatedAt,
            Items          = c.Items.Select(i => new ChecklistItemDto
            {
                ItemId      = i.ItemId,
                ChecklistId = i.ChecklistId,
                Text        = i.Text,
                IsCompleted = i.IsCompleted,
                Position    = i.Position,
                CreatedAt   = i.CreatedAt,
                UpdatedAt   = i.UpdatedAt
            }).ToList()
        };
    }
}