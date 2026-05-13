using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.CommentService.Entities;

[Table("Comments")]
public class Comment
{
    [Key]
    public int CommentId { get; set; }

    // Which card this comment belongs to
    public int CardId { get; set; }

    // Who wrote the comment (from JWT)
    public int UserId { get; set; }

    [Required, MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    // Track if comment was edited
    public bool IsEdited { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}