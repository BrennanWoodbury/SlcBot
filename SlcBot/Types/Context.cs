using Discord.WebSocket;

namespace SlcBot.Types;

public class Context
{
    public SocketGuild Guild { get; set; }
    public SocketUser? User { get; set; }
    public SocketGuildEvent? Event { get; set; }
}