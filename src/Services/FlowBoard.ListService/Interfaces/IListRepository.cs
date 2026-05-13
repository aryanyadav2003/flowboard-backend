using FlowBoard.ListService.Entities;

namespace FlowBoard.ListService.Interfaces;

public interface IListRepository
{
    Task<TaskList?>       GetByIdAsync(int listId);
    Task<List<TaskList>>  GetByBoardIdAsync(int boardId);
    Task<bool>            ExistsAsync(int listId);
    Task<int>             GetMaxPositionAsync(int boardId);
    Task<TaskList>        CreateAsync(TaskList list);
    Task<TaskList>        UpdateAsync(TaskList list);
    Task                  DeleteAsync(int listId);
    Task                  UpdatePositionsAsync(List<TaskList> lists);
}