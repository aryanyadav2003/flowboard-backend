using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.CardService.Entities;

[Table("CardAssignees")]
public class CardAssignee
{
    [Key]
    public int AssigneeId { get; set; }

    public int CardId { get; set; }

    // UserId from Auth Service
    public int UserId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("CardId")]
    public Card Card { get; set; } = null!;
}