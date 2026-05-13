using AutoMapper;
using FlowBoard.CommentService.DTOs;
using FlowBoard.CommentService.Entities;
using FlowBoard.CommentService.Interfaces;

namespace FlowBoard.CommentService.Services;

public class CommentServiceImpl : ICommentService
{
    private readonly ICommentRepository _repo;
    private readonly IMapper            _mapper;

    public CommentServiceImpl(ICommentRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<CommentDto> CreateAsync(CreateCommentDto dto, int userId)
    {
        var comment = new Comment
        {
            CardId    = dto.CardId,
            UserId    = userId,       // always from JWT — never trust client
            Content   = dto.Content,
            IsEdited  = false
        };

        await _repo.CreateAsync(comment);
        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<CommentDto> GetByIdAsync(int commentId)
    {
        var comment = await _repo.GetByIdAsync(commentId)
            ?? throw new KeyNotFoundException("Comment not found.");

        return _mapper.Map<CommentDto>(comment);
    }

    public async Task<List<CommentDto>> GetByCardIdAsync(int cardId)
    {
        var comments = await _repo.GetByCardIdAsync(cardId);
        return _mapper.Map<List<CommentDto>>(comments);
    }

    public async Task<List<CommentDto>> GetByUserIdAsync(int userId)
    {
        var comments = await _repo.GetByUserIdAsync(userId);
        return _mapper.Map<List<CommentDto>>(comments);
    }

    public async Task<CommentDto> UpdateAsync(
        int commentId, UpdateCommentDto dto, int userId)
    {
        var comment = await _repo.GetByIdAsync(commentId)
            ?? throw new KeyNotFoundException("Comment not found.");

        // Only the author can edit their comment
        if (comment.UserId != userId)
            throw new UnauthorizedAccessException(
                "You can only edit your own comments.");

        comment.Content   = dto.Content;
        comment.IsEdited  = true;         // mark as edited
        comment.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(comment);
        return _mapper.Map<CommentDto>(comment);
    }

    public async Task DeleteAsync(int commentId, int userId)
    {
        var comment = await _repo.GetByIdAsync(commentId)
            ?? throw new KeyNotFoundException("Comment not found.");

        // Only the author can delete their own comment
        if (comment.UserId != userId)
            throw new UnauthorizedAccessException(
                "You can only delete your own comments.");

        await _repo.DeleteAsync(commentId);
    }
}