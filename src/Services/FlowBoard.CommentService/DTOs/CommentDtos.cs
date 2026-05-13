using System.ComponentModel.DataAnnotations;

namespace FlowBoard.CommentService.DTOs;

public class CreateCommentDto
{
    [Required]
    public int CardId { get; set; }

    [Required, MaxLength(2000)]
    public string Content { get; set; } = "";
}

public class UpdateCommentDto
{
    [Required, MaxLength(2000)]
    public string Content { get; set; } = "";
}

public class CommentDto
{
    public int      CommentId { get; set; }
    public int      CardId    { get; set; }
    public int      UserId    { get; set; }
    public string   Content   { get; set; } = "";
    public bool     IsEdited  { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}