using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using VincenzoBot;
using VincenzoBot.Modules;
using VincenzoBot.Config;
using VincenzoBot.Repositories;
using Discord.Commands;

namespace VincenzoBot.Discord
{
    public class Connection
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordLogger _logger;
        private readonly CommandService _service;
        private readonly UserAccountRepository _userAccountRepository;
        private CommandHandlerService _commandHandler;
        private MessageHandlerService _messageHandler;
        private UserEventsHandlerService _userEventsHandler;
        public Connection(DiscordLogger logger, DiscordSocketClient client, UserAccountRepository userAccountRepository, CommandService service)
        {
            _service = service;
            _logger = logger;
            _client = client;
            _userAccountRepository = userAccountRepository;

        }
        internal async Task ConnectAsync(DiscordBotConfig config)
        {
            _commandHandler = new CommandHandlerService(_client, _logger, config, _service);
            _messageHandler = new MessageHandlerService(_client, _logger, config);
            _userEventsHandler = new UserEventsHandlerService(_client, _logger, config, _userAccountRepository);
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
            _userEventsHandler.Initialize();
            await Task.Delay(-1);
        }
    }
}
