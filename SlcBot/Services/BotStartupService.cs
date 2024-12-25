using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace SlcBot.Services;

public class BotStartupService
{
    private readonly DiscordSocketClient _client;
    private readonly BotService _botService;

    public BotStartupService(DiscordSocketClient client, BotService botService)
    {
        _client = client;
        _botService = botService;

        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
    }

    public async Task StartAsync(string token)
    {
        await _client.LoginAsync(TokenType.Bot,
            token);
        await _client.StartAsync();
    }

    private Task LogAsync(LogMessage msg)
    {
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }

    private Task ReadyAsync()
    {
        Console.WriteLine($"Connected as {_client.CurrentUser}");
        _botService.MainLoop();
        return Task.CompletedTask;
    }
}