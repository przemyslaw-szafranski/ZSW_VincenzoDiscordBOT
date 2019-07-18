using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Reflection;
using VincenzoBot.Discord;
using System;
using VincenzoBot.Config;
using Serilog;

namespace VincenzoBot
{
    class CommandHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly DiscordLogger _logger;
        private readonly DiscordBotConfig _config;
        private readonly IServiceProvider _serviceProvider;

        public CommandHandlerService(DiscordSocketClient client, DiscordLogger logger, DiscordBotConfig config, CommandService service, IServiceProvider serviceProvider)
        {
            _config = config;
            _client = client;
            _logger = logger;
            _service = service;
            _serviceProvider = serviceProvider;
        }
        public async Task InitializeAsync()
        {
            await _service.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: _serviceProvider);
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {

            if (arg.Channel is SocketDMChannel) { return; }
            if (arg.Author.IsBot) { return; }
            var msg = arg as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);
            int argPos = 0;
            if (msg.HasStringPrefix(_config.CmdPrefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) //if you use a prefix or mention a bot @Vincenzo command 
            {

                var result = await _service.ExecuteAsync(context, argPos, services: _serviceProvider);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand && result.Error != CommandError.UnmetPrecondition)
                {
                    Log.Error(new Exception(result.Error.ToString()), $"{context.User} ({context.Message}) - {result.ErrorReason}");
                }
                else if (!result.IsSuccess && result.Error == CommandError.UnmetPrecondition)
                {
                    Log.Warning(new Exception(result.Error.ToString()), $"{context.User} ({context.Message}) - {result.ErrorReason}");
                }
            }
        }
    }
}
