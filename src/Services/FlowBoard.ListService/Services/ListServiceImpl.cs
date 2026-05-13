using AutoMapper;
using FlowBoard.ListService.DTOs;
using FlowBoard.ListService.Entities;
using FlowBoard.ListService.Interfaces;

namespace FlowBoard.ListService.Services;

public class ListServiceImpl : IListService
{
    private readonly IListRepository _repo;
    private readonly IMapper         _mapper;

    public ListServiceImpl(IListRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<TaskListDto> CreateAsync(CreateListDto dto, int requesterId)
    {
        // Get max position and place new list at the end
        var maxPosition = await _repo.GetMaxPositionAsync(dto.BoardId);

        var list = new TaskList
        {
            Name     = dto.Name,
            BoardId  = dto.BoardId,
            Position = maxPosition + 1  // always appended at the end
        };

        await _repo.CreateAsync(list);
        return _mapper.Map<TaskListDto>(list);
    }

    public async Task<TaskListDto> GetByIdAsync(int listId)
    {
        var list = await _repo.GetByIdAsync(listId)
            ?? throw new KeyNotFoundException("List not found.");

        return _mapper.Map<TaskListDto>(list);
    }

    public async Task<List<TaskListDto>> GetByBoardIdAsync(int boardId)
    {
        var lists = await _repo.GetByBoardIdAsync(boardId);
        return _mapper.Map<List<TaskListDto>>(lists);
    }

    public async Task<TaskListDto> UpdateAsync(int listId, UpdateListDto dto)
    {
        var list = await _repo.GetByIdAsync(listId)
            ?? throw new KeyNotFoundException("List not found.");

        if (dto.Name     != null) list.Name     = dto.Name;
        if (dto.Position != null) list.Position = dto.Position.Value;
        list.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(list);
        return _mapper.Map<TaskListDto>(list);
    }

    public async Task<TaskListDto> MoveAsync(int listId, MoveListDto dto)
    {
        var list = await _repo.GetByIdAsync(listId)
            ?? throw new KeyNotFoundException("List not found.");

        var oldPosition = list.Position;
        var newPosition = dto.NewPosition;

        if (oldPosition == newPosition)
            return _mapper.Map<TaskListDto>(list);

        // Get all lists on the same board
        var allLists = await _repo.GetByBoardIdAsync(list.BoardId);

        // Shift other lists to make room
        foreach (var other in allLists)
        {
            if (other.ListId == listId) continue;

            // Moving right: shift left lists in between
            if (newPosition > oldPosition &&
                other.Position > oldPosition &&
                other.Position <= newPosition)
            {
                other.Position--;
            }
            // Moving left: shift right lists in between
            else if (newPosition < oldPosition &&
                     other.Position >= newPosition &&
                     other.Position < oldPosition)
            {
                other.Position++;
            }
        }

        list.Position  = newPosition;
        list.UpdatedAt = DateTime.UtcNow;

        // Save all shifted lists + the moved list together
        var toUpdate = allLists.Where(l => l.ListId != listId).ToList();
        toUpdate.Add(list);
        await _repo.UpdatePositionsAsync(toUpdate);

        return _mapper.Map<TaskListDto>(list);
    }

    public async Task ArchiveAsync(int listId)
    {
        var list = await _repo.GetByIdAsync(listId)
            ?? throw new KeyNotFoundException("List not found.");

        list.IsArchived = true;
        list.UpdatedAt  = DateTime.UtcNow;
        await _repo.UpdateAsync(list);

        // Re-number remaining lists on the board
        var remaining = await _repo.GetByBoardIdAsync(list.BoardId);
        for (int i = 0; i < remaining.Count; i++)
            remaining[i].Position = i;

        await _repo.UpdatePositionsAsync(remaining);
    }

    public async Task DeleteAsync(int listId)
    {
        var list = await _repo.GetByIdAsync(listId)
            ?? throw new KeyNotFoundException("List not found.");

        await _repo.DeleteAsync(listId);

        // Re-number remaining lists to fill the gap
        var remaining = await _repo.GetByBoardIdAsync(list.BoardId);
        for (int i = 0; i < remaining.Count; i++)
            remaining[i].Position = i;

        await _repo.UpdatePositionsAsync(remaining);
    }
}