using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using VincenzoBot.Config;
using VincenzoBot.Discord.Services.Commands.Preconditions;
using VincenzoBot.Preconditions;

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

        [Command("rules")]
        [DeleteCommandUsage]
        public async Task Rules()
        {
            var embed = new EmbedBuilder()
            {

                Title = "1.Nie Przeklinaj!\n" +
                        "2.Nie kradnij!\n" +
                        "3.Nie cudzołóż!\n" +
                        "4.Admin ma zawsze racje w szczególności ja!\n" +
                        "5.Nie spamuj!\n" +
                        "6.Zachowaj kulturę osobistą!\n" +
                        "7.Wykroczenia będą karane!\n" +
                        "8.Nie miej adminów cudzych przede mną!\n"

            };
            embed.WithColor(0xFF, 0xFF, 0x80);
            await Context.Channel.SendMessageAsync("8 przykazań Vincenzo:", false, embed.Build());
        }

        [Command("users")]
        [Cooldown(5)]
        [DeleteCommandUsage]
        public async Task HowManyMembers()
        {
            int count = 0;
            foreach (SocketGuildUser user in Context.Guild.Users)
            {
                if (!user.IsBot && !user.IsWebhook)
                    count++;
            }
            await Context.Channel.SendMessageAsync($"Serwer liczy {count} ludzi");
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

              var result = _service.Search(Context, query);
              if (query.StartsWith("module "))
                 query = query.Remove(0, "module ".Length);
              var emb = result.IsSuccess ? HelpCommand(result, builder) : await HelpModule(query, builder);

              if (emb.Fields.Length == 0)
              {
                  await ReplyAsync($"Sorry, I couldn't find anything for \"{query}\".");
                  return;
              }

              await Context.Channel.SendMessageAsync("", false, emb);*/
        }
    }
}
