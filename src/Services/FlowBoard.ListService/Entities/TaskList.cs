using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowBoard.ListService.Entities;

[Table("TaskLists")]
public class TaskList
{
    [Key]
    public int ListId { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public int BoardId { get; set; }

    // Position of this list on the board (0, 1, 2, 3...)
    public int Position { get; set; } = 0;

    public bool IsArchived { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}