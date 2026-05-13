using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.WorkspaceService.Entities;

[Table("WorkspaceMembers")]
public class WorkspaceMember
{
    [Key]
    public int MemberId { get; set; }

    public int WorkspaceId { get; set; }

    public int UserId { get; set; }

    [MaxLength(50)]
    public string Role { get; set; } = "MEMBER"; // OWNER | ADMIN | MEMBER

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("WorkspaceId")]
    public Workspace Workspace { get; set; } = null!;
}