using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.BoardService.Entities;

[Table("Boards")]
public class Board
{
    [Key]
    public int BoardId { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }

    [MaxLength(20)]
    public string Visibility { get; set; } = "PRIVATE"; // PRIVATE | PUBLIC

    public int WorkspaceId { get; set; }
    public int OwnerId     { get; set; }
    public bool IsArchived { get; set; } = false;
    public bool IsActive   { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();
}