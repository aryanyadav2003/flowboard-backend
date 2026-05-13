using System.ComponentModel.DataAnnotations;

namespace FlowBoard.LabelService.DTOs;

public class CreateLabelDto
{
    [Required]
    public int BoardId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(10)]
    public string Color { get; set; } = "#6366F1";
}

public class UpdateLabelDto
{
    [MaxLength(100)] public string? Name  { get; set; }
    [MaxLength(10)]  public string? Color { get; set; }
}

public class LabelDto
{
    public int      LabelId   { get; set; }
    public int      BoardId   { get; set; }
    public string   Name      { get; set; } = "";
    public string   Color     { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class CardLabelDto
{
    public int      CardLabelId { get; set; }
    public int      CardId      { get; set; }
    public int      LabelId     { get; set; }
    public string   LabelName   { get; set; } = "";
    public string   LabelColor  { get; set; } = "";
    public DateTime AssignedAt  { get; set; }
}

public class AssignLabelDto
{
    [Required] public int CardId { get; set; }
}