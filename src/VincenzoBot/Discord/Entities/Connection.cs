using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using VicenzoDiscordBot;
using VicenzoDiscordBot.Modules;
using VincenzoDiscordBot.Entities;

namespace VincenzoDiscordBot.Discord.Entities
{
    public class Connection
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordLogger _logger;
        private CommandHandlerService _commandHandler;
        private MessageHandlerService _messageHandler;
        public Connection(DiscordLogger logger, DiscordSocketClient client)
        {
            _logger = logger;
            _client = client;
        }
        internal async Task ConnectAsync(DiscordBotConfig config)
        {
            _commandHandler = new CommandHandlerService(_client, _logger, config);
            _messageHandler = new MessageHandlerService(_client, _logger, config);
            _client.Log += _logger.Log;
           // _client.Ready += Ready;
            if (config.Token == null || config.Token == "")
            {
                throw new ArgumentNullException("Token", "Discord Bot Token is empty!");
            }
            if (config.cmdPrefix == null || config.cmdPrefix == "")
            {
                throw new ArgumentNullException("cmdPrefix", "Discord Bot cmdPrefix is empty!");
            }
            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();
            await _commandHandler.InitializeAsync();
            _messageHandler.Initialize();
            await Task.Delay(-1);
        }
    }
}
