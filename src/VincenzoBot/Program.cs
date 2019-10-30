using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using VincenzoBot.Discord;

namespace VincenzoBot
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //Create list of dependencies
            IServiceProvider _serviceProvider = Startup.GetServiceProvider();
            _serviceProvider = Startup.BuildServiceProvider();
            var discordConnection = _serviceProvider.GetRequiredService<Connection>();
            ILogger logger = _serviceProvider.GetRequiredService<ILogger>();
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
