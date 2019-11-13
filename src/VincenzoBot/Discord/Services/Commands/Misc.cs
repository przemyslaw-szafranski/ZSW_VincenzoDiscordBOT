using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using VincenzoBot.Preconditions;
using VincenzoBot.Config;
using VincenzoBot.Discord.Services.Commands.Preconditions;
using System.Linq;
using System.Collections.Generic;

namespace VincenzoBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly DiscordBotConfig _config;

        public Misc(CommandService service)
        {
            _service = service;
        }

        [Cooldown(15)]
        [Command("help"), Alias("h"),
 Remarks(
     "DMs you a huge message if called without parameter - otherwise shows help to the provided command or module")]
        public async Task Help()
        {
            await Context.Channel.SendMessageAsync("Check your DMs.");

            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

            var contextString = Context.Guild?.Name ?? "DMs with me";
            var builder = new EmbedBuilder()
            {
                Title = "Help",
                Description = $"Oto lista komend, którą możesz używać na serwerze: {contextString}",
                Color = new Color(114, 137, 218)
            };

            foreach (var module in _service.Modules)
            {
                await AddModuleEmbedField(module, builder);
            }

            // We have a limit of 6000 characters for a message, so we are taking first ten fields
            // and then sending the message. In the current state it will send 2 messages.

            var _fieldRange = 10;
            var fields = builder.Fields.ToList();
            while (builder.Length > 6000)
            {
                builder.Fields.RemoveRange(0, fields.Count);
                var firstSet = fields.Take(_fieldRange);
                builder.Fields.AddRange(firstSet);
                if (builder.Length > 6000)
                {
                    _fieldRange--;
                    continue;
                }
                await dmChannel.SendMessageAsync("", false, builder.Build());
                fields.RemoveRange(0, _fieldRange);
                builder.Fields.RemoveRange(0, _fieldRange);
                builder.Fields.AddRange(fields);
            }

            await dmChannel.SendMessageAsync("", false, builder.Build());

            // Embed are limited to 24 Fields at max. So lets clear some stuff
            // out and then send it in multiple embeds if it is too big.
            builder.WithTitle("")
                .WithDescription("")
                .WithAuthor("");
            while (builder.Fields.Count > 24)
            {
                builder.Fields.RemoveRange(0, 25);
                await dmChannel.SendMessageAsync("", false, builder.Build());

            }
        }

        [RequireOwner]
        [Command("hi")]
        public async Task Hi()
        {
            if (_config.WelcomeMessageChannelID != 0)
            {
                var channel = Context.Guild.GetChannel(_config.WelcomeMessageChannelID) as ISocketMessageChannel;
                await channel.SendMessageAsync(Utilities.GetPhrase("welcomeMessage"));
            }
        }
        [Command("users")]
        [Cooldown(5)]
        [DeleteCommandUsage]
        public async Task HowManyMembers()
        {
            int count = 0;
            foreach(SocketGuildUser user in Context.Guild.Users)
            {
                if (!user.IsBot && !user.IsWebhook)
                    count++;
            }
            await Context.Channel.SendMessageAsync($"Serwer liczy {count} ludzi");
        }
        private async Task AddModuleEmbedField(ModuleInfo module, EmbedBuilder builder)
        {
            if (module is null) return;
            var descriptionBuilder = new List<string>();
            var duplicateChecker = new List<string>();
            foreach (var cmd in module.Commands)
            {
                var result = await cmd.CheckPreconditionsAsync(Context);
                if (!result.IsSuccess || duplicateChecker.Contains(cmd.Aliases.First())) continue;
                duplicateChecker.Add(cmd.Aliases.First());
                var cmdDescription = $"`{cmd.Aliases.First()}`";
                if (!string.IsNullOrEmpty(cmd.Summary))
                    cmdDescription += $" | {cmd.Summary}";
                if (!string.IsNullOrEmpty(cmd.Remarks))
                    cmdDescription += $" | {cmd.Remarks}";
                if (cmdDescription != "``")
                    descriptionBuilder.Add(cmdDescription);
            }

            if (descriptionBuilder.Count <= 0) return;
            var builtString = string.Join("\n", descriptionBuilder);
            var testLength = builtString.Length;
            if (testLength >= 1024)
            {
                throw new ArgumentException("Value cannot exceed 1024 characters");
            }
            var moduleNotes = "";
            if (!string.IsNullOrEmpty(module.Summary))
                moduleNotes += $" {module.Summary}";
            if (!string.IsNullOrEmpty(module.Remarks))
                moduleNotes += $" {module.Remarks}";
            if (!string.IsNullOrEmpty(moduleNotes))
                moduleNotes += "\n";
            if (!string.IsNullOrEmpty(module.Name))
            {
                builder.AddField($"__**{module.Name}:**__",
                    $"{moduleNotes} {builtString}\n ");
            }
        }
    }
}
