using AutoMapper;
using FlowBoard.CardService.DTOs;
using FlowBoard.CardService.Entities;
using FlowBoard.CardService.Interfaces;

namespace FlowBoard.CardService.Services;

public class CardServiceImpl : ICardService
{
    private readonly ICardRepository _repo;
    private readonly IMapper         _mapper;
    private readonly CacheService    _cache;

    public CardServiceImpl(
        ICardRepository repo,
        IMapper mapper,
        CacheService cache)
    {
        _repo   = repo;
        _mapper = mapper;
        _cache  = cache;
    }

    public async Task<CardDto> CreateAsync(CreateCardDto dto, int requesterId)
    {
        var maxPosition = await _repo.GetMaxPositionAsync(dto.ListId);

        var card = new Card
        {
            Title           = dto.Title,
            Description     = dto.Description,
            ListId          = dto.ListId,
            BoardId         = dto.BoardId,
            Priority        = dto.Priority,
            DueDate         = dto.DueDate,
            CreatedByUserId = requesterId,
            Position        = maxPosition + 1
        };

        await _repo.CreateAsync(card);

        // Invalidate list and board cache
        await _cache.RemoveAsync(CacheService.CardsByListKey(dto.ListId));
        await _cache.RemoveAsync(CacheService.CardsByBoardKey(dto.BoardId));

        return MapToDto(card);
    }

    public async Task<CardDto> GetByIdAsync(int cardId)
    {
        // Try cache first
        var cacheKey = CacheService.CardKey(cardId);
        var cached   = await _cache.GetAsync<CardDto>(cacheKey);
        if (cached != null)
        {
            return cached; // ← returned from Redis, no DB hit
        }

        // Cache miss — go to DB
        var card = await _repo.GetByIdWithAssigneesAsync(cardId)
            ?? throw new KeyNotFoundException("Card not found.");

        var dto = MapToDto(card);

        // Store in cache for 10 minutes
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

        return dto;
    }

    public async Task<List<CardDto>> GetByListIdAsync(int listId)
    {
        // Try cache first
        var cacheKey = CacheService.CardsByListKey(listId);
        var cached   = await _cache.GetAsync<List<CardDto>>(cacheKey);
        if (cached != null) return cached;

        // Cache miss — go to DB
        var cards = await _repo.GetByListIdAsync(listId);
        var dtos  = cards.Select(MapToDto).ToList();

        // Cache for 5 minutes
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5));

        return dtos;
    }

    public async Task<List<CardDto>> GetByBoardIdAsync(int boardId)
    {
        // Try cache first
        var cacheKey = CacheService.CardsByBoardKey(boardId);
        var cached   = await _cache.GetAsync<List<CardDto>>(cacheKey);
        if (cached != null) return cached;

        // Cache miss — go to DB
        var cards = await _repo.GetByBoardIdAsync(boardId);
        var dtos  = cards.Select(MapToDto).ToList();

        // Cache for 5 minutes
        await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5));

        return dtos;
    }

    public async Task<List<CardDto>> GetOverdueAsync()
    {
        // Overdue cards change frequently — no caching
        var cards = await _repo.GetOverdueCardsAsync();
        return cards.Select(MapToDto).ToList();
    }

    public async Task<CardDto> UpdateAsync(int cardId, UpdateCardDto dto, int requesterId)
    {
        var card = await _repo.GetByIdWithAssigneesAsync(cardId)
            ?? throw new KeyNotFoundException("Card not found.");

        if (card.CreatedByUserId != requesterId)
            throw new UnauthorizedAccessException("Only the creator can update this card.");

        if (dto.Title       != null) card.Title       = dto.Title;
        if (dto.Description != null) card.Description = dto.Description;
        if (dto.Status      != null) card.Status      = dto.Status;
        if (dto.Priority    != null) card.Priority    = dto.Priority;
        if (dto.DueDate     != null) card.DueDate     = dto.DueDate;
        card.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(card);

        // Invalidate all related caches
        await InvalidateCardCaches(card.CardId, card.ListId, card.BoardId);

        return MapToDto(card);
    }

    public async Task<CardDto> MoveAsync(int cardId, MoveCardDto dto, int requesterId)
    {
        var card = await _repo.GetByIdWithAssigneesAsync(cardId)
            ?? throw new KeyNotFoundException("Card not found.");
        
        // Allow moves if they are the creator
        if (card.CreatedByUserId != requesterId)
            throw new UnauthorizedAccessException("Only the creator can move this card.");

        var oldListId   = card.ListId;
        var oldPosition = card.Position;
        var newListId   = dto.ListId;
        var newPosition = dto.NewPosition;

        if (oldListId != newListId)
        {
            var oldListCards = await _repo.GetByListIdAsync(oldListId);
            var oldFiltered  = oldListCards
                .Where(c => c.CardId != cardId && c.Position > oldPosition)
                .ToList();
            foreach (var c in oldFiltered) c.Position--;
            await _repo.UpdatePositionsAsync(oldFiltered);

            var newListCards = await _repo.GetByListIdAsync(newListId);
            var newFiltered  = newListCards
                .Where(c => c.Position >= newPosition)
                .ToList();
            foreach (var c in newFiltered) c.Position++;
            await _repo.UpdatePositionsAsync(newFiltered);

            card.ListId   = newListId;
            card.Position = newPosition;
        }
        else
        {
            if (oldPosition == newPosition)
                return MapToDto(card);

            var allCards = await _repo.GetByListIdAsync(oldListId);
            foreach (var other in allCards)
            {
                if (other.CardId == cardId) continue;
                if (newPosition > oldPosition &&
                    other.Position > oldPosition &&
                    other.Position <= newPosition)
                    other.Position--;
                else if (newPosition < oldPosition &&
                         other.Position >= newPosition &&
                         other.Position < oldPosition)
                    other.Position++;
            }
            card.Position = newPosition;
            var toUpdate  = allCards.Where(c => c.CardId != cardId).ToList();
            await _repo.UpdatePositionsAsync(toUpdate);
        }

        card.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(card);

        // Invalidate old list, new list and board caches
        await _cache.RemoveAsync(CacheService.CardsByListKey(oldListId));
        await _cache.RemoveAsync(CacheService.CardsByListKey(newListId));
        await _cache.RemoveAsync(CacheService.CardsByBoardKey(card.BoardId));
        await _cache.RemoveAsync(CacheService.CardKey(cardId));

        return MapToDto(card);
    }

    public async Task ArchiveAsync(int cardId, int requesterId)
    {
        var card = await _repo.GetByIdAsync(cardId)
            ?? throw new KeyNotFoundException("Card not found.");

        if (card.CreatedByUserId != requesterId)
            throw new UnauthorizedAccessException("Only the creator can archive this card.");

        card.IsArchived = true;
        card.UpdatedAt  = DateTime.UtcNow;
        await _repo.UpdateAsync(card);

        var remaining = await _repo.GetByListIdAsync(card.ListId);
        for (int i = 0; i < remaining.Count; i++)
            remaining[i].Position = i;
        await _repo.UpdatePositionsAsync(remaining);

        await InvalidateCardCaches(cardId, card.ListId, card.BoardId);
    }

    public async Task DeleteAsync(int cardId, int requesterId)
    {
        var card = await _repo.GetByIdAsync(cardId)
            ?? throw new KeyNotFoundException("Card not found.");

        if (card.CreatedByUserId != requesterId)
            throw new UnauthorizedAccessException("Only the creator can delete this card.");

        var listId  = card.ListId;
        var boardId = card.BoardId;

        await _repo.DeleteAsync(cardId);

        var remaining = await _repo.GetByListIdAsync(listId);
        for (int i = 0; i < remaining.Count; i++)
            remaining[i].Position = i;
        await _repo.UpdatePositionsAsync(remaining);

        await InvalidateCardCaches(cardId, listId, boardId);
    }

    public async Task<CardAssigneeDto> AssignUserAsync(
        int cardId, AssignUserDto dto, int requesterId)
    {
        var card = await _repo.GetByIdAsync(cardId)
            ?? throw new KeyNotFoundException("Card not found.");

        if (card.CreatedByUserId != requesterId)
            throw new UnauthorizedAccessException("Only the creator can manage assignees.");

        if (await _repo.IsAssignedAsync(cardId, dto.UserId))
            throw new InvalidOperationException("User is already assigned.");

        var assignee = new CardAssignee
        {
            CardId = cardId,
            UserId = dto.UserId
        };

        await _repo.AddAssigneeAsync(assignee);

        // Invalidate card cache
        await _cache.RemoveAsync(CacheService.CardKey(cardId));

        return _mapper.Map<CardAssigneeDto>(assignee);
    }

    public async Task UnassignUserAsync(int cardId, int userId, int requesterId)
    {
        var card = await _repo.GetByIdAsync(cardId)
            ?? throw new KeyNotFoundException("Card not found.");

        if (card.CreatedByUserId != requesterId)
            throw new UnauthorizedAccessException("Only the creator can manage assignees.");

        await _repo.RemoveAssigneeAsync(cardId, userId);

        // Invalidate card cache
        await _cache.RemoveAsync(CacheService.CardKey(cardId));
    }

    public async Task<List<CardAssigneeDto>> GetAssigneesAsync(int cardId)
    {
        if (!await _repo.ExistsAsync(cardId))
            throw new KeyNotFoundException("Card not found.");

        var assignees = await _repo.GetAssigneesAsync(cardId);
        return _mapper.Map<List<CardAssigneeDto>>(assignees);
    }

    public async Task<List<CardDto>> GetMyTasksAsync(int userId)
    {
        var cards = await _repo.GetCardsByAssigneeAsync(userId);
        return cards.Select(MapToDto).ToList();
    }

    // ── Helpers ───────────────────────────────────────────────
    private async Task InvalidateCardCaches(
        int cardId, int listId, int boardId)
    {
        await _cache.RemoveAsync(CacheService.CardKey(cardId));
        await _cache.RemoveAsync(CacheService.CardsByListKey(listId));
        await _cache.RemoveAsync(CacheService.CardsByBoardKey(boardId));
    }

    private static CardDto MapToDto(Card c) => new()
    {
        CardId          = c.CardId,
        Title           = c.Title,
        Description     = c.Description,
        ListId          = c.ListId,
        BoardId         = c.BoardId,
        Position        = c.Position,
        Status          = c.Status,
        Priority        = c.Priority,
        CreatedByUserId = c.CreatedByUserId,
        DueDate         = c.DueDate,
        IsArchived      = c.IsArchived,
        IsOverdue       = c.DueDate.HasValue &&
                          c.DueDate.Value < DateTime.UtcNow &&
                          c.Status != "DONE",
        CreatedAt       = c.CreatedAt,
        UpdatedAt       = c.UpdatedAt,
        Assignees       = c.Assignees.Select(a => new CardAssigneeDto
        {
            AssigneeId = a.AssigneeId,
            CardId     = a.CardId,
            UserId     = a.UserId,
            AssignedAt = a.AssignedAt
        }).ToList()
    };
}