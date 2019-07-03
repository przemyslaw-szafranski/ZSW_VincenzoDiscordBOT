namespace VincenzoBot.Config
{
    public class DiscordBotConfig
    {
        public string Token { get; set; } = null;
        public string CmdPrefix { get; set; } = null;
        public ulong GuildID { get; set; } = 0;
        public ulong WelcomeMessageChannelID { get; set; } = 0;
        public ulong CommandsOutputChannelID { get; set; } = 0;

    }
}
