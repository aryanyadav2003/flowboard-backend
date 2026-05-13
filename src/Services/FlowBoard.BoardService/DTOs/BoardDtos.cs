using System.ComponentModel.DataAnnotations;

namespace FlowBoard.BoardService.DTOs;

public class CreateBoardDto
{
    [Required, MaxLength(150)] public string  Name         { get; set; } = "";
    [MaxLength(500)]           public string? Description  { get; set; }
    [MaxLength(500)]           public string? CoverImageUrl { get; set; }
    [MaxLength(20)]            public string  Visibility   { get; set; } = "PRIVATE";
    [Required]                 public int     WorkspaceId  { get; set; }
}

public class UpdateBoardDto
{
    [MaxLength(150)] public string? Name          { get; set; }
    [MaxLength(500)] public string? Description   { get; set; }
    [MaxLength(500)] public string? CoverImageUrl { get; set; }
    [MaxLength(20)]  public string? Visibility    { get; set; }
}

public class BoardDto
{
    public int     BoardId       { get; set; }
    public string  Name          { get; set; } = "";
    public string? Description   { get; set; }
    public string? CoverImageUrl { get; set; }
    public string  Visibility    { get; set; } = "";
    public int     WorkspaceId   { get; set; }
    public int     OwnerId       { get; set; }
    public bool    IsArchived    { get; set; }
    public bool    IsActive      { get; set; }
    public int     MemberCount   { get; set; }
    public DateTime CreatedAt    { get; set; }
    public DateTime UpdatedAt    { get; set; }
}

public class BoardMemberDto
{
    public int      BoardMemberId { get; set; }
    public int      BoardId       { get; set; }
    public int      UserId        { get; set; }
    public string   Role          { get; set; } = "";
    public DateTime JoinedAt      { get; set; }
}

public class AddBoardMemberDto
{
    [Required]      public int    UserId { get; set; }
    [MaxLength(50)] public string Role   { get; set; } = "MEMBER";
}

public class UpdateBoardMemberRoleDto
{
    [Required, MaxLength(50)] public string Role { get; set; } = "";
}