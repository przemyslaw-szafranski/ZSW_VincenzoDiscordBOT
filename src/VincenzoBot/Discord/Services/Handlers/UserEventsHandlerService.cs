using Discord.WebSocket;
using System.Threading.Tasks;
using VincenzoBot.Discord;
using VincenzoBot.Config;
using VincenzoBot.Repositories;

namespace VincenzoBot
{
    class UserEventsHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordLogger _logger;
        private readonly DiscordBotConfig _config;
        private readonly IUserAccountRepository _userAccountRepository;
        public UserEventsHandlerService(DiscordSocketClient client, DiscordLogger logger, DiscordBotConfig config, IUserAccountRepository userAccountRepository)
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
            _client.UserUpdated += HandleUserUpdatedAsync;

        }
        private async Task HandleUserTypingAsync(SocketUser user, ISocketMessageChannel channel)
        {
            //await channel.SendMessageAsync("uj");
        }
        private async Task HandleUserJoinedAsync(SocketGuildUser gUser)
        {
            if (gUser.IsBot || gUser.IsWebhook) return;
            SocketUser user = gUser;
            await _userAccountRepository.CreateUserAccountAsync(user);
        }
        private async Task HandleUserUpdatedAsync(SocketUser user, SocketGuild guild)
        {
            if (user.IsBot || user.IsWebhook) return;
                _userAccountRepository.RemoveUser(user.Id);
        }
        private async Task HandleUserBannedAsync(SocketUser user, SocketGuild guild)
        {
            if (user.IsBot || user.IsWebhook) return;
            _userAccountRepository.RemoveUser(user.Id);
        }
        private async Task HandleUserUpdatedAsync(SocketUser arg1, SocketUser arg2)
        {
            var oldUser = arg1 as SocketGuildUser;
            var newUser = arg2 as SocketGuildUser;
            if (oldUser.Username != newUser.Username)
                _userAccountRepository.UpdateUserFileAndNickname(oldUser);
        }
    }
}
