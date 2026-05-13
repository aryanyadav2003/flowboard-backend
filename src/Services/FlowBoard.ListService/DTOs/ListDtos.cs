using System.ComponentModel.DataAnnotations;

namespace FlowBoard.ListService.DTOs;

public class CreateListDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = "";

    [Required]
    public int BoardId { get; set; }

    public int Position { get; set; } = 0;
}

public class UpdateListDto
{
    [MaxLength(150)]
    public string? Name { get; set; }

    public int? Position { get; set; }
}

public class MoveListDto
{
    [Required]
    public int NewPosition { get; set; }
}

public class TaskListDto
{
    public int      ListId     { get; set; }
    public string   Name       { get; set; } = "";
    public int      BoardId    { get; set; }
    public int      Position   { get; set; }
    public bool     IsArchived { get; set; }
    public DateTime CreatedAt  { get; set; }
    public DateTime UpdatedAt  { get; set; }
}