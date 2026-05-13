using FlowBoard.BoardService.Entities;

namespace FlowBoard.BoardService.Interfaces;

public interface IBoardRepository
{
    Task<Board?>            GetByIdAsync(int boardId);
    Task<Board?>            GetByIdWithMembersAsync(int boardId);
    Task<List<Board>>       GetByWorkspaceIdAsync(int workspaceId);
    Task<List<Board>>       GetByMemberIdAsync(int userId);
    Task<List<Board>>       GetPublicAsync();
    Task<bool>              ExistsAsync(int boardId);
    Task<Board>             CreateAsync(Board board);
    Task<Board>             UpdateAsync(Board board);
    Task                    DeleteAsync(int boardId);

    Task<BoardMember?>      GetMemberAsync(int boardId, int userId);
    Task<List<BoardMember>> GetMembersAsync(int boardId);
    Task<bool>              IsMemberAsync(int boardId, int userId);
    Task<BoardMember>       AddMemberAsync(BoardMember member);
    Task<BoardMember>       UpdateMemberAsync(BoardMember member);
    Task                    RemoveMemberAsync(int boardId, int userId);
}