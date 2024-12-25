namespace SlcBot.Data.Entities;

public class EventAttendee: BaseEntity
{
    public Guid ServerEventId { get; set; }
    public ulong DiscordEventId { get; set; }
    public ulong UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsCreator { get; set; }
    public bool HasResponded { get; set; }

    public ServerEvent ServerEvent { get; set; } = null!;
}