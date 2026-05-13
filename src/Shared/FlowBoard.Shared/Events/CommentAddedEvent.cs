namespace FlowBoard.Shared.Events;

public record CommentAddedEvent
{
    public int    CommentId          { get; init; }
    public int    CardId             { get; init; }
    public string CardTitle          { get; init; } = string.Empty;
    public int    ActorId            { get; init; }
    public string Content            { get; init; } = string.Empty;
    public List<int> MentionedUserIds { get; init; } = new();
    public DateTime OccurredAt       { get; init; } = DateTime.UtcNow;
}