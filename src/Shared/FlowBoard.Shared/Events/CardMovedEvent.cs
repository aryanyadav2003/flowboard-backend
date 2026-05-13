namespace FlowBoard.Shared.Events;

public record CardMovedEvent
{
    public int    CardId      { get; init; }
    public int    BoardId     { get; init; }
    public string CardTitle   { get; init; } = string.Empty;
    public int    ActorId     { get; init; }
    public int    FromListId  { get; init; }
    public int    ToListId    { get; init; }
    public string ToListName  { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}