using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SlcBot.Data.Contexts;
using SlcBot.Data.Entities;
using SlcBot.Types;

namespace SlcBot.Services;

public class BotService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;
    private Context Context = new Context();
    public ICollection<SocketGuildEvent> ServerEvents = new List<SocketGuildEvent>();
    public ICollection<SocketGuildUser> Users = new List<SocketGuildUser>();

    public BotService(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    // TODO
    // Need to add events to the database
    // Need to add users going to the event to the database <- we can do this at the time we want to get RSVPs
    // Need to figure out how to see new users going to the event
    public async Task MainLoop()
    {
        string guildIdString = _configuration["Discord:GuildId"]
                               ?? throw new Exception("GuildId not found in appsettings.json");
        ulong guildId = ulong.Parse(guildIdString);
        SetGuildContext(guildId);


        while (true)
        {
            GetNewEvents(guildId);

            foreach (SocketGuildEvent item in ServerEvents)
            {
                ServerEvent? serverEvent = _dbContext.ServerEvents.FirstOrDefault(x => x.DiscordEventId == item.Id);
                if (item.Id == serverEvent?.DiscordEventId)
                {
                    Console.WriteLine("Event already exists in database");
                    if (serverEvent.StartTime != item.StartTime)
                    {
                        Console.WriteLine("Event time has changed");
                        serverEvent.StartTime = item.StartTime;
                        serverEvent.EndTime = item.EndTime;
                        _dbContext.SaveChanges();
                    }

                    if (serverEvent.EndTime != item.EndTime)
                    {
                        Console.WriteLine("Event time has changed");
                        serverEvent.StartTime = item.StartTime;
                        serverEvent.EndTime = item.EndTime;
                        _dbContext.SaveChanges();
                    }

                    if (serverEvent.Description != item.Description)
                    {
                        Console.WriteLine("Event description has changed");
                        serverEvent.Description = item.Description;
                        _dbContext.SaveChanges();
                    }

                    if (serverEvent.Title != item.Name)
                    {
                        Console.WriteLine("Event title has changed");
                        serverEvent.Title = item.Name;
                        _dbContext.SaveChanges();
                    }
                }

                else if (serverEvent == null)
                {
                    _dbContext.ServerEvents.Add(new ServerEvent
                    {
                        Id = Guid.NewGuid(),
                        Title = item.Name,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        DiscordEventId = item.Id,
                        Description = item.Description,
                        CreatorId = item.Creator.Id,
                        CreatorName = item.Creator.Username,
                        GuildId = item.GuildId
                    });

                    _dbContext.SaveChanges();
                }



                if (item.StartTime > DateTimeOffset.Now + TimeSpan.FromHours(12)) continue;
                if (item.StartTime < DateTime.Now)
                {
                    Console.WriteLine("Event has already started");
                    continue;
                }

                SetEventContext(item);
                Console.WriteLine($"New Event Context: {item.Name}");
                Context.Event = item;
                Context.User = item.Creator;

                var attendees = await _dbContext.EventAttendees
                    .Where(x => x.DiscordEventId == item.Id)
                    .ToListAsync();
                foreach (RestUser? user in await item.GetUsersAsync(null).FlattenAsync())
                {
                    foreach (var attendee in attendees)
                    {
                        if (user.Id == attendee.UserId) continue;

                    }
                }

            }

            // Thread.Sleep(3000);
            Thread.Sleep(300000);
        }


    }

    public IReadOnlyCollection<SocketGuild> GetGuilds()
    {
        foreach (SocketGuild? guild in _client.Guilds)
        {
            Console.WriteLine(guild.Name);
            Console.WriteLine(guild.Id);
        }
        return _client.Guilds;
    }

    private SocketGuild GetGuild(ulong id)
    {
        return _client.GetGuild(id);
    }

    private SocketGuildUser GetUser(ulong guildId, ulong userId)
    {
        return GetGuild(guildId).GetUser(userId);
    }

    protected void GetNewEvents(ulong guildId)
    {
        foreach (var guildEvent in Context.Guild.Events)
        {
            if (ServerEvents.Contains(guildEvent)) continue;
            ServerEvents.Add(guildEvent);
        }
    }



    protected void SetGuildContext(ulong id)
    {
        Context.Guild = GetGuild(id);
    }

    protected void SetUserContext(ulong guildId, ulong userId)
    {
        Context.User = GetUser(guildId, userId);
    }

    protected void SetEventContext(SocketGuildEvent guildEvent)
    {
        Context.Event = guildEvent;
    }

    protected void ClearContext()
    {
        Context = new Context();
    }

    protected void ClearEvents()
    {
        ServerEvents = new List<SocketGuildEvent>();
    }


}