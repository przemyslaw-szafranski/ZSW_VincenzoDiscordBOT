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
using VincenzoBot.Repositories;

namespace VincenzoBot.Modules
{
    public class MessageHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordLogger _logger;
        private readonly DiscordBotConfig _config;
        private readonly LevelingService _levelingService;
        private readonly UserAccountRepository _userRepo;
        public MessageHandlerService(DiscordSocketClient client, DiscordLogger logger, DiscordBotConfig config, LevelingService levelingService, UserAccountRepository userRepo)
        {
            _config = config;
            _client = client;
            _logger = logger;
            _levelingService = levelingService;
            _userRepo = userRepo;
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
            int argPos = 0;
            var user = _userRepo.GetOrCreateUser(arg.Author);
            if (!msg.HasStringPrefix(_config.CmdPrefix, ref argPos))
                await _levelingService.RewardMessage(user, arg.Content);
            if(await _levelingService.LevelUp(user))
            {
                await msg.Channel.SendMessageAsync($"Gratulacje {user.Nickname}, wbiłeś level {user.Level}");
                return;
            }
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
