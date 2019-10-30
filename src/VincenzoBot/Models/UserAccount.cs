using System;

namespace VincenzoDiscordBot.Models
{
    public class UserAccount
    {
        public ulong Id { get; set; }
        public string Nickname { get; set; }
        public DateTime LastDaily { get; set; } = DateTime.UtcNow.AddDays(-2);
        public DateTime LastMessage { get; set; } = DateTime.UtcNow;
        public string Yt_id { get; set; }
        //last visit date
        public uint Xp { get; set; }
        public uint Haczyks { get; set; } // my currency ֏
                                          //number of warns

    }
}
