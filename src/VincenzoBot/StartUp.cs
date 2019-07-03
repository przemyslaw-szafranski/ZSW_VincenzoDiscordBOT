using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using VincenzoBot.Config;
using VincenzoBot.Repositories;
using VincenzoBot.Storages;
using VincenzoBot.Storages.Implementations;

namespace VincenzoBot.Discord
{
    public class Startup
    {
        public static IServiceProvider BuildServiceProvider() => new ServiceCollection()
            //Global
            .AddSingleton<ILogger, Logger>()
            .AddSingleton<IDataStorage, JsonStorage>()
            //Discord Stuff
            .AddSingleton<DiscordBotConfig>()
            .AddSingleton<BotConfigRepository>()
            .AddSingleton<Connection>()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<DiscordLogger>()
            .AddSingleton<UserAccountRepository>()
            // You can pass in an instance of the desired type

            // ...or by using the generic method.
            //
            // The benefit of using the generic method is that 
            // ASP.NET DI will attempt to inject the required
            // dependencies that are specified under the constructor 
            // for us.
            .BuildServiceProvider();
    }
}
