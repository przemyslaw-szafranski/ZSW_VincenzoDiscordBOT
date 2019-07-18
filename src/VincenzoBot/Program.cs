using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;
using VincenzoBot.Discord;

namespace VincenzoBot
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .WriteTo.Console(
                LogEventLevel.Verbose,
                "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();
            try
            {
                //Create list of dependencies
                IServiceProvider _serviceProvider = Discord.ServiceProvider.GetServiceProvider();
                _serviceProvider = Discord.ServiceProvider.BuildServiceProvider();
                var discordConnection = _serviceProvider.GetRequiredService<Connection>();
                ILogger logger = _serviceProvider.GetRequiredService<ILogger>();
                var discordBotConfig = _serviceProvider.GetRequiredService<BotConfigRepository>();
                try
                {
                    await discordConnection.ConnectAsync(discordBotConfig._config);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                   // logger.Log("EXCEPTION: " + e.Message);
                }
            }
            finally
            {
                Log.CloseAndFlush();
            }
            Console.Write("Click any button to exit...");
            Console.ReadKey();
        }

    }
   
}
