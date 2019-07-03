using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using VincenzoBot.Discord;

namespace VincenzoBot
{
    internal class Program
    {
        private static IServiceProvider _serviceProvider;
        private static async Task Main(string[] args)
        {
            //Create list of dependencies
            _serviceProvider = Startup.BuildServiceProvider();
            // Unity.RegisterTypes();
            var discordConnection = _serviceProvider.GetRequiredService<Connection>();
            var logger = _serviceProvider.GetRequiredService<ILogger>();
            // var userAccountRepository = Unity.Resolve<VincenzoDiscordBot.Repositories.UserAccountRepository>();
             var discordBotConfig = _serviceProvider.GetRequiredService<BotConfigRepository>();
            try
            {
                await discordConnection.ConnectAsync(discordBotConfig._config);

            }
            catch(Exception e)
            {
                logger.Log("EXCEPTION: " + e.Message);
            }

            Console.Write("Click any button to exit...");
            Console.ReadKey();
        }

    }
   
}
