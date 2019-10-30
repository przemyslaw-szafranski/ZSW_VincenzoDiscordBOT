using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Reflection;
using VincenzoDiscordBot.Discord;
using System;
using VincenzoDiscordBot.Entities;
using VincenzoDiscordBot.Repositories;

namespace VicenzoDiscordBot
{
    class UserEventsHandlerService
    {
        private readonly DiscordSocketClient _client;
        private CommandService _service;
        private readonly DiscordLogger _logger;
        private readonly DiscordBotConfig _config;
        private readonly UserAccountRepository _userAccountRepository;
        public UserEventsHandlerService(DiscordSocketClient client, DiscordLogger logger, DiscordBotConfig config, UserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
            _config = config;
            _client = client;
            _logger = logger;

        }
        public void Initialize()
        {
            _client.UserIsTyping += HandleUserTypingAsync;
            _client.UserJoined += HandleUserJoinedAsync;
            _client.UserBanned += HandleUserBannedAsync;
            //_client.UserUpdated;
        }
        private async Task HandleUserTypingAsync(SocketUser user, ISocketMessageChannel channel)
        {
            //await channel.SendMessageAsync("uj");
        }
        private async Task HandleUserJoinedAsync(SocketGuildUser gUser)
        {
            if (gUser.IsBot || gUser.IsWebhook) return;
            SocketUser user = gUser;
            _userAccountRepository.CreateUserAccount(user);
        }
        private async Task HandleUserBannedAsync(SocketUser user, SocketGuild guild)
        {
            if (user.IsBot || user.IsWebhook) return;
            _userAccountRepository.RemoveUser(user.Id);
        }



    }
}
