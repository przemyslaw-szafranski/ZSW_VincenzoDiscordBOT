using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Reflection;
using VincenzoDiscordBot.Discord;
using System;
using VincenzoDiscordBot.Entities;

namespace VicenzoDiscordBot
{
    class CommandHandlerService
    {
        private readonly DiscordSocketClient _client;
        private CommandService _service;
        private readonly DiscordLogger _logger;
        private readonly DiscordBotConfig _config;

        public CommandHandlerService(DiscordSocketClient client, DiscordLogger logger, DiscordBotConfig config)
        {
            _config = config;
            _client = client;
            _logger = logger;

        }
        public async Task InitializeAsync()
        {
            _service = new CommandService();
            await _service.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),services: null);
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
            if(msg.HasStringPrefix(_config.cmdPrefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) //if you use a prefix or mention a bot @Vincenzo command 
            {
                var result = await _service.ExecuteAsync(context, argPos,services:null);
                if(!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    throw new ArgumentException("Unknown command");
                }
            }
        }

    }
}
