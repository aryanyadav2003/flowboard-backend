using FlowBoard.CommentService.Data;
using FlowBoard.CommentService.Entities;
using FlowBoard.CommentService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlowBoard.CommentService.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly CommentDbContext _db;
    public CommentRepository(CommentDbContext db) => _db = db;

    public Task<Comment?> GetByIdAsync(int commentId)
        => _db.Comments.FindAsync(commentId).AsTask();

    public Task<List<Comment>> GetByCardIdAsync(int cardId)
        => _db.Comments
              .Where(c => c.CardId == cardId)
              .OrderBy(c => c.CreatedAt)
              .ToListAsync();

    public Task<List<Comment>> GetByUserIdAsync(int userId)
        => _db.Comments
              .Where(c => c.UserId == userId)
              .OrderByDescending(c => c.CreatedAt)
              .ToListAsync();

    public Task<bool> ExistsAsync(int commentId)
        => _db.Comments.AnyAsync(c => c.CommentId == commentId);

    public async Task<Comment> CreateAsync(Comment comment)
    {
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment> UpdateAsync(Comment comment)
    {
        _db.Comments.Update(comment);
        await _db.SaveChangesAsync();
        return comment;
    }

    public async Task DeleteAsync(int commentId)
    {
        var comment = await GetByIdAsync(commentId);
        if (comment != null)
        {
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
        }
    }
}