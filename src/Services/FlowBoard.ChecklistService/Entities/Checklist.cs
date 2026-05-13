using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.ChecklistService.Entities;

[Table("Checklists")]
public class Checklist
{
    [Key]
    public int ChecklistId { get; set; }

    // Which card this checklist belongs to
    public int CardId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation — items inside this checklist
    public ICollection<ChecklistItem> Items { get; set; }
        = new List<ChecklistItem>();
}