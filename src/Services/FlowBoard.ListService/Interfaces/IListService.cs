using FlowBoard.ListService.DTOs;

namespace FlowBoard.ListService.Interfaces;

public interface IListService
{
    Task<TaskListDto>       CreateAsync(CreateListDto dto, int requesterId);
    Task<TaskListDto>       GetByIdAsync(int listId);
    Task<List<TaskListDto>> GetByBoardIdAsync(int boardId);
    Task<TaskListDto>       UpdateAsync(int listId, UpdateListDto dto);
    Task<TaskListDto>       MoveAsync(int listId, MoveListDto dto);
    Task                    ArchiveAsync(int listId);
    Task                    DeleteAsync(int listId);
}