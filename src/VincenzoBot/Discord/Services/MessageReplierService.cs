using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace VicenzoDiscordBot.Modules
{
    public class MessageReplierService
    {
        private readonly DiscordSocketClient _client;

        public MessageReplierService(DiscordSocketClient client)
        {
            _client = client;
        }
        public string checkMessage(SocketMessage socketMsg)
        {
            if (socketMsg.Content.Contains("kocham"))
                return $"*Vincenzo śmieje się z {socketMsg.Author.Username}.*";
            return "";
        }
    }
}
