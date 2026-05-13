using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.LabelService.Entities;

[Table("CardLabels")]
public class CardLabel
{
    [Key]
    public int CardLabelId { get; set; }

    // Which card the label is assigned to
    public int CardId { get; set; }

    // Which label is assigned
    public int LabelId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("LabelId")]
    public Label Label { get; set; } = null!;
}