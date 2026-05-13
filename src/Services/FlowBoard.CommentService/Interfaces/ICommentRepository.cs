using FlowBoard.CommentService.Entities;

namespace FlowBoard.CommentService.Interfaces;

public interface ICommentRepository
{
    Task<Comment?>       GetByIdAsync(int commentId);
    Task<List<Comment>>  GetByCardIdAsync(int cardId);
    Task<List<Comment>>  GetByUserIdAsync(int userId);
    Task<bool>           ExistsAsync(int commentId);
    Task<Comment>        CreateAsync(Comment comment);
    Task<Comment>        UpdateAsync(Comment comment);
    Task                 DeleteAsync(int commentId);
}