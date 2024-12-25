namespace SlcBot.Data.Entities;

public class ServerEvent: BaseEntity
{
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ulong CreatorId { get; set; }
    public string CreatorName { get; set; } = null!;
    public ulong GuildId { get; set; }
    public ulong DiscordEventId { get; set; }

    public List<EventAttendee> Attendees { get; set; } = new();
}