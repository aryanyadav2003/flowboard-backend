using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.ChecklistService.Entities;

[Table("ChecklistItems")]
public class ChecklistItem
{
    [Key]
    public int ItemId { get; set; }

    // Which checklist this item belongs to
    public int ChecklistId { get; set; }

    [Required, MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    // Is this item completed or not
    public bool IsCompleted { get; set; } = false;

    // Order of item inside checklist
    public int Position { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ChecklistId")]
    public Checklist Checklist { get; set; } = null!;
}