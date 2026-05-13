using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.CardService.Entities;

[Table("Cards")]
public class Card
{
    [Key]
    public int CardId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    // Which list this card belongs to
    public int ListId { get; set; }

    // Which board this card belongs to (for quick lookup)
    public int BoardId { get; set; }

    // Position inside the list (0, 1, 2...)
    public int Position { get; set; } = 0;

    [MaxLength(20)]
    public string Status { get; set; } = "TO_DO";
    // TO_DO | IN_PROGRESS | IN_REVIEW | DONE

    [MaxLength(20)]
    public string Priority { get; set; } = "MEDIUM";
    // LOW | MEDIUM | HIGH | CRITICAL

    // Who created this card
    public int CreatedByUserId { get; set; }

    // Optional due date
    public DateTime? DueDate { get; set; }

    public bool IsArchived { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation — assignees of this card
    public ICollection<CardAssignee> Assignees { get; set; }
        = new List<CardAssignee>();
}