using System.ComponentModel.DataAnnotations;

namespace FlowBoard.WorkspaceService.DTOs;

public class CreateWorkspaceDto
{
    [Required, MaxLength(150)] public string  Name        { get; set; } = "";
    [MaxLength(500)]           public string? Description { get; set; }
    [MaxLength(500)]           public string? LogoUrl     { get; set; }
    [MaxLength(20)]            public string  Visibility  { get; set; } = "PRIVATE";
}

public class UpdateWorkspaceDto
{
    [MaxLength(150)] public string?  Name        { get; set; }
    [MaxLength(500)] public string?  Description { get; set; }
    [MaxLength(500)] public string?  LogoUrl     { get; set; }
    [MaxLength(20)]  public string?  Visibility  { get; set; }
}

public class WorkspaceDto
{
    public int     WorkspaceId  { get; set; }
    public string  Name         { get; set; } = "";
    public string? Description  { get; set; }
    public string? LogoUrl      { get; set; }
    public string  Visibility   { get; set; } = "";
    public int     OwnerId      { get; set; }
    public bool    IsActive     { get; set; }
    public int     MemberCount  { get; set; }
    public DateTime CreatedAt   { get; set; }
    public DateTime UpdatedAt   { get; set; }
}

public class WorkspaceMemberDto
{
    public int      MemberId    { get; set; }
    public int      WorkspaceId { get; set; }
    public int      UserId      { get; set; }
    public string   Role        { get; set; } = "";
    public DateTime JoinedAt    { get; set; }
}

public class AddMemberDto
{
    [Required] public int    UserId { get; set; }
    [MaxLength(50)] public string Role   { get; set; } = "MEMBER";
}

public class UpdateMemberRoleDto
{
    [Required, MaxLength(50)] public string Role { get; set; } = "";
}