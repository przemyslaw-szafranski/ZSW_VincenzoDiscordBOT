using Discord;
using Moq;
using System;
using System.Threading.Tasks;
using VincenzoBot;

namespace VincenzoBot.Discord
{
    public class DiscordLogger
    {
        ILogger _logger;
        public DiscordLogger(ILogger logger)
        {
            _logger = logger;
        }
        public Task Log(LogMessage logMsg)
        {
            _logger.Log(logMsg.Message,"Discord");
            return Task.CompletedTask;
        }
        public Task Log(string logMsg)
        {
            _logger.Log(logMsg, "Discord");
            return Task.CompletedTask;
        }

        public static implicit operator DiscordLogger(Mock<DiscordLogger> v)
        {
            return v;
        }
    }
}
