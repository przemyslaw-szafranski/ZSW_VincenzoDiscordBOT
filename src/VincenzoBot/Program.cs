using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VicenzoDiscordBot;
using VicenzoDiscordBot.Modules;
using VincenzoBot.Storages;
using VincenzoDiscordBot;
using VincenzoDiscordBot.Discord.Entities;
using VincenzoDiscordBot.Repositories;

namespace VincenzoBot
{
    internal class Program
    {
        private static async Task DiscordBotThread(Connection discordConnection, BotConfigRepository discordBotConfig)
        {
            await discordConnection.ConnectAsync(discordBotConfig._config);
        }
        private static async Task Main(string[] args)
        {
            Unity.RegisterTypes();
           // var storage = Unity.Resolve<IDataStorage>();
            var logger = Unity.Resolve<ILogger>();
            var discordConnection = Unity.Resolve<Connection>();
            var userAccountRepository = Unity.Resolve<VincenzoDiscordBot.Repositories.UserAccountRepository>();
            var discordBotConfig = Unity.Resolve<BotConfigRepository>();
            try
            {
                // Thread thread1 = new Thread(async () => await DiscordBotThread(discordConnection, discordBotConfig));
                //thread1.Start();
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
