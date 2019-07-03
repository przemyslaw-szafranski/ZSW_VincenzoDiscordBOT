using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using VincenzoBot.Preconditions;
using VincenzoBot.Config;

namespace VincenzoBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
       // private readonly CommandService _service;
        private readonly DiscordBotConfig _config;
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
        public async Task HowManyMembers()
        {
            await Context.Channel.SendMessageAsync($"Serwer liczy {Context.Guild.MemberCount} ludzi");
        }
        [Command("help"), Alias("h")]
        [Remarks("Shows what a specific command or module does and what parameters it takes.")]
        [Cooldown(5)]
        public async Task HelpQuery([Remainder] string query)
        {
          /*  var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Title = $"Help for '{query}'"
            };

          //  var result = _service.Search(Context, query);
            if (query.StartsWith("module "))
                query = query.Remove(0, "module ".Length);
          //  var emb = result.IsSuccess ? HelpCommand(result, builder) : await HelpModule(query, builder);

            if (emb.Fields.Length == 0)
            {
                await ReplyAsync($"Sorry, I couldn't find anything for \"{query}\".");
                return;
            }

            await Context.Channel.SendMessageAsync("", false, emb);*/
        }
    }
}
