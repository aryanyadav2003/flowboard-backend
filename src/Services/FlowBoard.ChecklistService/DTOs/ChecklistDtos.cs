using System.ComponentModel.DataAnnotations;

namespace FlowBoard.ChecklistService.DTOs;

public class CreateChecklistDto
{
    [Required]
    public int CardId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";
}

public class UpdateChecklistDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = "";
}

public class CreateChecklistItemDto
{
    [Required, MaxLength(500)]
    public string Text { get; set; } = "";
}

public class UpdateChecklistItemDto
{
    [MaxLength(500)]
    public string? Text { get; set; }
}

public class ChecklistDto
{
    public int                  ChecklistId  { get; set; }
    public int                  CardId       { get; set; }
    public string               Title        { get; set; } = "";
    public int                  TotalItems   { get; set; }
    public int                  CompletedItems { get; set; }
    public int                  Progress     { get; set; } // 0-100 percentage
    public DateTime             CreatedAt    { get; set; }
    public DateTime             UpdatedAt    { get; set; }
    public List<ChecklistItemDto> Items      { get; set; } = new();
}

public class ChecklistItemDto
{
    public int      ItemId      { get; set; }
    public int      ChecklistId { get; set; }
    public string   Text        { get; set; } = "";
    public bool     IsCompleted { get; set; }
    public int      Position    { get; set; }
    public DateTime CreatedAt   { get; set; }
    public DateTime UpdatedAt   { get; set; }
}