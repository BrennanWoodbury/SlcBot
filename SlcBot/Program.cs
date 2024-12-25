using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SlcBot.Data.Contexts;
using SlcBot.Services;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("B:\\code\\dotnet\\SlcBot\\SlcBot\\appsettings.json")
    .Build();

static IServiceProvider CreateProvider(IConfiguration configuration)
{
    // Configure services
    var services = new ServiceCollection();

    var config = new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildScheduledEvents
    };

    services.AddSingleton(config);
    services.AddSingleton(configuration);
    services.AddSingleton<DiscordSocketClient>();
    services.AddSingleton<BotStartupService>();
    services.AddSingleton<BotService>();

    services.AddDbContext<ApplicationDbContext>(x =>
    {
        x.UseNpgsql(configuration["Database:ConnectionString"]
                    ?? throw new Exception("ConnectionString not found in appsettings.json"));

    });

    return services.BuildServiceProvider();
}

IServiceProvider serviceProvider = CreateProvider(configuration);

var botService = serviceProvider.GetRequiredService<BotStartupService>();
var applicationDbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

await botService.StartAsync(configuration["Discord:Token"]
                            ?? throw new Exception("Token not found in appsettings.json"));

await Task.Delay(Timeout.Infinite);


