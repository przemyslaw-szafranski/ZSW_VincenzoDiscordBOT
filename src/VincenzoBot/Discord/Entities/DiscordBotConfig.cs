namespace VincenzoDiscordBot.Entities
{
    public class DiscordBotConfig
    {
        public string Token { get; set; } = null;
        //public DiscordSocketConfig SocketConfig { get; set; }
        // public string YTclientId { get; set; }
        // public string YTclientSecret { get; set; }
        //public string YTstreamerAccessToken { get; set; }
        // public string YTBotAccessToken { get; set; }
        public string cmdPrefix { get; set; } = null;
        public ulong welcomeMessageChannelID { get; set; } = 0;
        public ulong commandsOutputChannelID { get; set; } = 0;

    }
}
