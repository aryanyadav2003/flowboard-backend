using FlowBoard.CommentService.DTOs;

namespace FlowBoard.CommentService.Interfaces;

public interface ICommentService
{
    Task<CommentDto>       CreateAsync(CreateCommentDto dto, int userId);
    Task<CommentDto>       GetByIdAsync(int commentId);
    Task<List<CommentDto>> GetByCardIdAsync(int cardId);
    Task<List<CommentDto>> GetByUserIdAsync(int userId);
    Task<CommentDto>       UpdateAsync(int commentId, UpdateCommentDto dto, int userId);
    Task                   DeleteAsync(int commentId, int userId);
}