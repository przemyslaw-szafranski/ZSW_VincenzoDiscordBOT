using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using VincenzoBot.Discord;
using VincenzoBot.Config;

namespace VincenzoBot.Modules
{
    public class MessageHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordLogger _logger;
        private readonly DiscordBotConfig _config;

        public MessageHandlerService(DiscordSocketClient client, DiscordLogger logger, DiscordBotConfig config)
        {
            _config = config;
            _client = client;
            _logger = logger;

        }
        public void Initialize()
        {
            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage arg)
        {
            if (arg.Channel is SocketDMChannel) { return; }
            if (arg.Author.IsBot) { return; }
            var msg = arg as SocketUserMessage;
            if (msg == null) return;
            string respond = checkMessage(arg);
            if (!respond.Equals(""))
                await msg.Channel.SendMessageAsync(respond);
        }

        private string checkMessage(SocketMessage socketMsg)
        {
            if (socketMsg.Content.Contains("kocham"))
                return $"*Vincenzo śmieje się z {socketMsg.Author.Username}.*";
            return "";
        }
    }
}
