using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.BoardService.Entities;

[Table("BoardMembers")]
public class BoardMember
{
    [Key]
    public int BoardMemberId { get; set; }

    public int BoardId { get; set; }
    public int UserId  { get; set; }

    [MaxLength(50)]
    public string Role { get; set; } = "MEMBER"; // OWNER | ADMIN | MEMBER | VIEWER

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("BoardId")]
    public Board Board { get; set; } = null!;
}