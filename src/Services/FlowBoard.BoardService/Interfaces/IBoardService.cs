using FlowBoard.BoardService.DTOs;

namespace FlowBoard.BoardService.Interfaces;

public interface IBoardService
{
    Task<BoardDto>             CreateAsync(int ownerId, CreateBoardDto dto);
    Task<BoardDto>             GetByIdAsync(int boardId, int requesterId);
    Task<List<BoardDto>>       GetByWorkspaceAsync(int workspaceId, int requesterId);
    Task<List<BoardDto>>       GetMyBoardsAsync(int userId);
    Task<List<BoardDto>>       GetPublicBoardsAsync();
    Task<BoardDto>             UpdateAsync(int boardId, int requesterId, UpdateBoardDto dto);
    Task                       ArchiveBoardAsync(int boardId, int requesterId);
    Task                       DeleteAsync(int boardId, int requesterId);

    Task<BoardMemberDto>       AddMemberAsync(int boardId, int requesterId, AddBoardMemberDto dto);
    Task<List<BoardMemberDto>> GetMembersAsync(int boardId, int requesterId);
    Task<BoardMemberDto>       UpdateMemberRoleAsync(int boardId, int targetUserId, int requesterId, UpdateBoardMemberRoleDto dto);
    Task                       RemoveMemberAsync(int boardId, int targetUserId, int requesterId);
    Task                       LeaveAsync(int boardId, int userId);
}