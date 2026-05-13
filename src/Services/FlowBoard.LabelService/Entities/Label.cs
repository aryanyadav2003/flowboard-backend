using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.LabelService.Entities;

[Table("Labels")]
public class Label
{
    [Key]
    public int LabelId { get; set; }

    // Labels belong to a board
    public int BoardId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Hex color code e.g. "#FF0000"
    [Required, MaxLength(10)]
    public string Color { get; set; } = "#6366F1";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation to card assignments
    public ICollection<CardLabel> CardLabels { get; set; }
        = new List<CardLabel>();
}