using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using VicenzoBot.Modules;
using VincenzoBot.Config;
using VincenzoBot.Modules;
using VincenzoBot.Repositories;
using VincenzoBot.Services.Discord;
using VincenzoBot.Storages;
using VincenzoBot.Storages.Implementations;

namespace VincenzoBot.Discord
{
    public class ServiceProvider
    {
        private readonly static IServiceProvider _serviceProvider;

        public static IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }
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
            .AddSingleton<CommandHandlerService>()
            .AddSingleton<DiscordLogger>()
            .AddSingleton<IUserAccountRepository,UserAccountRepository>()
            .AddSingleton<UserAccountService>()
            .AddSingleton<MessageHandlerService>()
            .AddSingleton<LevelingService>()
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
