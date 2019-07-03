using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using VincenzoBot.Repositories;

namespace VincenzoBot.Discord
{
    public class VincenzoCommandContext:SocketCommandContext
    {
        private readonly UserAccountRepository _userRepository;

        public VincenzoCommandContext(DiscordSocketClient client, SocketUserMessage msg, UserAccountRepository userRepository) : base(client, msg)
        {
            this._userRepository = userRepository;
        }

        //public void RegisterCommandUsage()
        //{
        //    var commandUsedInformation = new CommandInformation(Message.Content, Message.CreatedAt.DateTime);

        //    UserAccount.AddCommandToHistory(commandUsedInformation);

        //    _globalUserAccounts.SaveAccounts(UserAccount.Id);
        //}
    }
}
