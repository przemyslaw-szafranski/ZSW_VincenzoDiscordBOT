﻿using System;

namespace VincenzoBot.Models
{
    public class UserAccount
    {
        public ulong Id { get; set; }
        public string Nickname { get; set; }
        public DateTime LastDaily { get; set; } = DateTime.UtcNow.AddDays(-1);
        public DateTime LastMessage { get; set; } = DateTime.UtcNow;
        public string Yt_id { get; set; } = null;
        //last visit date
        public uint Level { get; set; } = 0;
        public uint Xp { get; set; } = 0;
        public uint Haczyks { get; set; } = Constants.WELCOME_HACZYKS;// my currency ֏
                                          //number of warns

    }
}
