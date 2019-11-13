using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VincenzoBot.Config;
using VincenzoBot.Discord;
using VincenzoBot.Discord.Models;
using VincenzoBot.Repositories;

namespace VincenzoBot.Modules
{
    public class MessageHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordLogger _logger;
        private readonly DiscordBotConfig _config;
        private readonly LevelingService _levelingService;
        private readonly IUserAccountRepository _userRepo;

        private List<Message> _messagesList;
        public MessageHandlerService(DiscordSocketClient client, DiscordLogger logger, DiscordBotConfig config, LevelingService levelingService, IUserAccountRepository userRepo)
        {
            _config = config;
            _client = client;
            _logger = logger;
            _levelingService = levelingService;
            _userRepo = userRepo;
            _messagesList = new List<Message>();
        }
        public void Initialize()
        {
            _client.MessageReceived += HandleMessageAsync;
            _client.UserJoined += AnnounceJoinedUser;
        }

        private async Task HandleMessageAsync(SocketMessage arg)
        {
            try
            {
                if (arg.Channel is SocketDMChannel) { return; }
                if (arg.Author.IsBot) { return; }
                var msg = arg as SocketUserMessage;
                if (msg == null) return;
                int argPos = 0;
                var user = _userRepo.GetOrCreateUserAsync(arg.Author);
                if (!msg.HasStringPrefix(_config.CmdPrefix, ref argPos))
                    await _levelingService.RewardMessage(user.Result, arg.Content);
                if (await _levelingService.LevelUp(user.Result))
                {
                    await msg.Channel.SendMessageAsync($"Gratulacje {user.Result.Nickname}, wbiłeś level {user.Result.Level}");
                    return;
                }
                string respond = CheckMessage(arg);
                if (!respond.Equals(""))
                    await msg.Channel.SendMessageAsync(respond);

                respond = CheckIfVulgarity(arg);
                if (!respond.Equals(""))
                {
                    var message = msg as IMessage;
                    await message.DeleteAsync();
                    await msg.Channel.SendMessageAsync(respond);
                }

                respond = CheckIfSpam(arg);
                if (!respond.Equals(""))
                {
                    var message = msg as IMessage;
                    await message.DeleteAsync();
                    await msg.Channel.SendMessageAsync(respond);
                }
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        private string CheckMessage(SocketMessage socketMsg)
        {
            if (socketMsg.Content.Contains("kocham"))
                return $"*Vincenzo śmieje się z {socketMsg.Author.Username}.*";
            return "";
        }

        private string CheckIfVulgarity(SocketMessage socketMsg)
        {
            var vulgarityList = File.ReadAllLines(Constants.VULAGARITY_LIST_PATH).ToList();
            if (vulgarityList.Any(x => socketMsg.Content.Contains(x)))
                return $"*Tylko Vincenzo może tutaj przeklinać {socketMsg.Author.Username}.*";
            return "";
        }

        private string CheckIfSpam(SocketMessage socketMsg)
        {
            _messagesList.Add(new Message()
            {
                DateOfSending = socketMsg.CreatedAt.UtcDateTime,
                UserId = socketMsg.Author.Id,
            });

            _messagesList = _messagesList.Where(x => (DateTime.UtcNow - x.DateOfSending).TotalSeconds < 5).ToList();

            if (_messagesList.Count(x => x.UserId == socketMsg.Author.Id) > Constants.MAX_MESSAGES_PER_5_SECONDS)
                return $"*Tylko Vincenzo może tutaj spamować {socketMsg.Author.Username}.*";
            return "";
        }

        public async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            try
            {
                var channel = _client.GetChannel(631895437543997444) as SocketTextChannel;
                await channel.SendMessageAsync("Witaj " + user.Mention + " na serwerze!");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
