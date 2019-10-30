using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using VincenzoBot;
using VincenzoBot.Modules;
using VincenzoBot.Config;
using VincenzoBot.Repositories;
using Discord.Commands;
using Serilog;

namespace VincenzoBot.Discord
{
    public class Connection
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordLogger _logger;
        private readonly CommandService _service;
        private readonly IUserAccountRepository _userAccountRepository;
        private CommandHandlerService _commandHandler;
        private MessageHandlerService _messageHandler;
        private UserEventsHandlerService _userEventsHandler;
        private readonly IServiceProvider _serviceProvider;
        private readonly LevelingService _levelingService;
        public Connection(DiscordLogger logger, DiscordSocketClient client, IUserAccountRepository userAccountRepository,
            CommandService service, IServiceProvider serviceProvider, LevelingService levelingService)
        {
            _service = service;
            _logger = logger;
            _client = client;
            _levelingService = levelingService;
            _userAccountRepository = userAccountRepository;
            _serviceProvider = serviceProvider;

        }
        internal async Task ConnectAsync(DiscordBotConfig config)
        {
            Log.Information("Initializing connection.");
            _commandHandler = new CommandHandlerService(_client, _logger, config, _service, _serviceProvider);
            _messageHandler = new MessageHandlerService(_client, _logger, config, _levelingService, _userAccountRepository);
            _userEventsHandler = new UserEventsHandlerService(_client, _logger, config, _userAccountRepository);
            _client.Log += _logger.Log;
            _client.Ready += ReadyAsync;
            if (config.Token == null || config.Token == "")
                throw new ArgumentNullException("Token", "Discord Bot Token is empty!");
            if (config.CmdPrefix == null || config.CmdPrefix == "")
                throw new ArgumentNullException("cmdPrefix", "Discord Bot cmdPrefix is empty!");

            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();
            await _commandHandler.InitializeAsync();
            _messageHandler.Initialize();
            _userEventsHandler.Initialize();
            await Task.Delay(-1);

        }

        private async Task<Task> ReadyAsync()
        {
            Log.Information("Connected.");
            Log.Information("Synchronizing user accounts...");
            foreach (var guild in _client.Guilds)
            {
                foreach (var user in guild.Users)
                {
                    if (!user.IsBot && !user.IsWebhook)
                        await _userAccountRepository.GetOrCreateUserAsync(user);
                }
            }
            Log.Information("Synchronized user accounts.");
            return Task.CompletedTask;
        }
    }
}
