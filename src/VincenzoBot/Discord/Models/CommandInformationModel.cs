using System;

namespace CommunityBot.Entities
{
    public class CommandInformationModel
    {
        public string Command { get; set; }
        public DateTime UsageDate { get; set; } = DateTime.Now;

        public CommandInformationModel()
        {
        }

        public CommandInformationModel(string command, DateTime usageDate)
        {
            Command = command;
            UsageDate = usageDate;
        }
    }
}
