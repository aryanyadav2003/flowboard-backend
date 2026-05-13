using AutoMapper;
using FlowBoard.BoardService.DTOs;
using FlowBoard.BoardService.Entities;
using FlowBoard.BoardService.Interfaces;

namespace FlowBoard.BoardService.Services;

public class BoardServiceImpl : IBoardService
{
    private readonly IBoardRepository _repo;
    private readonly IMapper          _mapper;

    public BoardServiceImpl(IBoardRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<BoardDto> CreateAsync(int ownerId, CreateBoardDto dto)
    {
        var board = new Board
        {
            Name         = dto.Name,
            Description  = dto.Description,
            CoverImageUrl = dto.CoverImageUrl,
            Visibility   = dto.Visibility,
            WorkspaceId  = dto.WorkspaceId,
            OwnerId      = ownerId
        };

        await _repo.CreateAsync(board);

        // Auto-add owner as OWNER member
        await _repo.AddMemberAsync(new BoardMember
        {
            BoardId  = board.BoardId,
            UserId   = ownerId,
            Role     = "OWNER"
        });

        var created = await _repo.GetByIdWithMembersAsync(board.BoardId);
        return MapToDto(created!);
    }

    public async Task<BoardDto> GetByIdAsync(int boardId, int requesterId)
    {
        var board = await _repo.GetByIdWithMembersAsync(boardId)
            ?? throw new KeyNotFoundException("Board not found.");

        if (board.Visibility == "PRIVATE" &&
            !await _repo.IsMemberAsync(boardId, requesterId))
            throw new UnauthorizedAccessException("Access denied to private board.");

        return MapToDto(board);
    }

    public async Task<List<BoardDto>> GetByWorkspaceAsync(int workspaceId, int requesterId)
    {
        var boards = await _repo.GetByWorkspaceIdAsync(workspaceId);
        return boards
            .Where(b => b.Visibility == "PUBLIC" ||
                        b.Members.Any(m => m.UserId == requesterId))
            .Select(MapToDto).ToList();
    }

    public async Task<List<BoardDto>> GetMyBoardsAsync(int userId)
    {
        var boards = await _repo.GetByMemberIdAsync(userId);
        return boards.Select(MapToDto).ToList();
    }

    public async Task<List<BoardDto>> GetPublicBoardsAsync()
    {
        var boards = await _repo.GetPublicAsync();
        return boards.Select(MapToDto).ToList();
    }

    public async Task<BoardDto> UpdateAsync(int boardId, int requesterId, UpdateBoardDto dto)
    {
        var board = await _repo.GetByIdWithMembersAsync(boardId)
            ?? throw new KeyNotFoundException("Board not found.");

        await EnsureAdminOrOwner(boardId, requesterId);

        if (dto.Name          != null) board.Name          = dto.Name;
        if (dto.Description   != null) board.Description   = dto.Description;
        if (dto.CoverImageUrl != null) board.CoverImageUrl = dto.CoverImageUrl;
        if (dto.Visibility    != null) board.Visibility    = dto.Visibility;
        board.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(board);
        return MapToDto(board);
    }

    public async Task ArchiveBoardAsync(int boardId, int requesterId)
    {
        var board = await _repo.GetByIdAsync(boardId)
            ?? throw new KeyNotFoundException("Board not found.");

        await EnsureAdminOrOwner(boardId, requesterId);

        board.IsArchived = true;
        board.UpdatedAt  = DateTime.UtcNow;
        await _repo.UpdateAsync(board);
    }

    public async Task DeleteAsync(int boardId, int requesterId)
    {
        var board = await _repo.GetByIdAsync(boardId)
            ?? throw new KeyNotFoundException("Board not found.");

        if (board.OwnerId != requesterId)
            throw new UnauthorizedAccessException("Only the owner can delete this board.");

        board.IsActive  = false;
        board.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(board);
    }

    public async Task<BoardMemberDto> AddMemberAsync(int boardId, int requesterId, AddBoardMemberDto dto)
    {
        await EnsureAdminOrOwner(boardId, requesterId);

        if (await _repo.IsMemberAsync(boardId, dto.UserId))
            throw new InvalidOperationException("User is already a member.");

        var member = new BoardMember
        {
            BoardId = boardId,
            UserId  = dto.UserId,
            Role    = dto.Role
        };

        await _repo.AddMemberAsync(member);
        return _mapper.Map<BoardMemberDto>(member);
    }

    public async Task<List<BoardMemberDto>> GetMembersAsync(int boardId, int requesterId)
    {
        if (!await _repo.IsMemberAsync(boardId, requesterId))
            throw new UnauthorizedAccessException("Access denied.");

        var members = await _repo.GetMembersAsync(boardId);
        return _mapper.Map<List<BoardMemberDto>>(members);
    }

    public async Task<BoardMemberDto> UpdateMemberRoleAsync(int boardId, int targetUserId, int requesterId, UpdateBoardMemberRoleDto dto)
    {
        await EnsureAdminOrOwner(boardId, requesterId);

        var member = await _repo.GetMemberAsync(boardId, targetUserId)
            ?? throw new KeyNotFoundException("Member not found.");

        member.Role = dto.Role;
        await _repo.UpdateMemberAsync(member);
        return _mapper.Map<BoardMemberDto>(member);
    }

    public async Task RemoveMemberAsync(int boardId, int targetUserId, int requesterId)
    {
        await EnsureAdminOrOwner(boardId, requesterId);

        if (!await _repo.IsMemberAsync(boardId, targetUserId))
            throw new KeyNotFoundException("Member not found.");

        await _repo.RemoveMemberAsync(boardId, targetUserId);
    }

    public async Task LeaveAsync(int boardId, int userId)
    {
        var board = await _repo.GetByIdAsync(boardId)
            ?? throw new KeyNotFoundException("Board not found.");

        if (board.OwnerId == userId)
            throw new InvalidOperationException("Owner cannot leave. Transfer ownership first.");

        await _repo.RemoveMemberAsync(boardId, userId);
    }

    // ── Helpers ──────────────────────────────────────────────
    private async Task EnsureAdminOrOwner(int boardId, int userId)
    {
        var member = await _repo.GetMemberAsync(boardId, userId)
            ?? throw new UnauthorizedAccessException("You are not a member of this board.");

        if (member.Role != "OWNER" && member.Role != "ADMIN")
            throw new UnauthorizedAccessException("Only admins or owners can perform this action.");
    }

    private static BoardDto MapToDto(Board b) => new()
    {
        BoardId       = b.BoardId,
        Name          = b.Name,
        Description   = b.Description,
        CoverImageUrl = b.CoverImageUrl,
        Visibility    = b.Visibility,
        WorkspaceId   = b.WorkspaceId,
        OwnerId       = b.OwnerId,
        IsArchived    = b.IsArchived,
        IsActive      = b.IsActive,
        MemberCount   = b.Members.Count,
        CreatedAt     = b.CreatedAt,
        UpdatedAt     = b.UpdatedAt
    };
}