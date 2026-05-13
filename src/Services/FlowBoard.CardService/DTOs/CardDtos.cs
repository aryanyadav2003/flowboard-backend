using System.ComponentModel.DataAnnotations;

namespace FlowBoard.CardService.DTOs;

public class CreateCardDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public int ListId { get; set; }

    [Required]
    public int BoardId { get; set; }

    [MaxLength(20)]
    public string Priority { get; set; } = "MEDIUM";

    public DateTime? DueDate { get; set; }
}

public class UpdateCardDto
{
    [MaxLength(200)]  public string?   Title       { get; set; }
    [MaxLength(2000)] public string?   Description { get; set; }
    [MaxLength(20)]   public string?   Status      { get; set; }
    [MaxLength(20)]   public string?   Priority    { get; set; }
                      public DateTime? DueDate     { get; set; }
}

public class MoveCardDto
{
    [Required] public int ListId      { get; set; }
    [Required] public int NewPosition { get; set; }
}

public class AssignUserDto
{
    [Required] public int UserId { get; set; }
}

public class CardDto
{
    public int       CardId          { get; set; }
    public string    Title           { get; set; } = "";
    public string?   Description     { get; set; }
    public int       ListId          { get; set; }
    public int       BoardId         { get; set; }
    public int       Position        { get; set; }
    public string    Status          { get; set; } = "";
    public string    Priority        { get; set; } = "";
    public int       CreatedByUserId { get; set; }
    public DateTime? DueDate         { get; set; }
    public bool      IsArchived      { get; set; }
    public bool      IsOverdue       { get; set; }
    public DateTime  CreatedAt       { get; set; }
    public DateTime  UpdatedAt       { get; set; }
    public List<CardAssigneeDto> Assignees { get; set; } = new();
}

public class CardAssigneeDto
{
    public int      AssigneeId { get; set; }
    public int      CardId     { get; set; }
    public int      UserId     { get; set; }
    public DateTime AssignedAt { get; set; }
}