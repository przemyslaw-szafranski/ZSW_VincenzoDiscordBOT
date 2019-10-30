namespace VincenzoBot.Config
{
    public class DiscordBotConfig
    {
        public string Token { get; set; } = null;
        public string cmdPrefix { get; set; } = null;
        public ulong welcomeMessageChannelID { get; set; } = 0;
        public ulong commandsOutputChannelID { get; set; } = 0;

    }
}
