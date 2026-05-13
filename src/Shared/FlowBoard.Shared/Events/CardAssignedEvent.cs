namespace FlowBoard.Shared.Events;

public record CardAssignedEvent
{
    public int    CardId      { get; init; }
    public int    BoardId     { get; init; }
    public string CardTitle   { get; init; } = string.Empty;
    public int    ActorId     { get; init; }
    public int    AssigneeId  { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}