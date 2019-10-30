using System;
using System.Collections.Generic;
using System.Text;

namespace VincenzoBot.Discord.Entities
{
    public class Warn
    {
        enum Strenght
        {
            low,
            medium,
            high
        }
        public string reason { get; set; } = null;
        //public Strenght strenght { get; set; };

    }
}
