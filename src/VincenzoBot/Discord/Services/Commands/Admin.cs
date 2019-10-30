using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VincenzoBot.Preconditions;
using VincenzoBot.Repositories;
using VincenzoBot.Services.Discord;

namespace VincenzoBot.Modules
{
    //Commands which are about user information
    public class Admin : ModuleBase<SocketCommandContext>
    {
        private readonly UserAccountRepository _userRepo;
        public Admin(UserAccountRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [Command("saveAccounts"), Alias("saccounts", "savea")]
        public async Task SaveAccounts()
        {
            await Context.Message.DeleteAsync();
            await _userRepo.SaveAccounts();
            await Context.Channel.SendMessageAsync("*Vincenzo zamyka kartotekę i chowa do szuflady*\nZapisałem ich dane... Którego odstrzelimy?");
        }
        //TODO update czyjes yt
        //TODO logger tych komend
        [Command("announce")]
        [Remarks("Make A Announcement")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Announce([Remainder]string announcement)
        {
            var embed = EmbedHandlerService.CreateEmbed("Announcement By " + Context.Message.Author, announcement, EmbedHandlerService.EmbedMessageType.Info, true);

            await Context.Channel.SendMessageAsync("", false, embed);
            await Context.Message.DeleteAsync();
        }
        [Command("Game"), Alias("SetGame")]
        [Remarks("Change what the bot is currently playing.")]
        [RequireOwner]
        public async Task SetGame([Remainder] string gamename)
        {
          //  await Context.Client.SetGameAsync(gamename);
          //  var channel = Context.Guild.GetChannel(BotConfigRepository.Config.commandsOutputChannelID) as ISocketMessageChannel;
          //  await channel.SendMessageAsync($"Changed game to `{gamename}`");
        }

        [Command("purge")]
        [Remarks("Purges A User's Last Messages. Default Amount To Purge Is 100")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Clear(SocketGuildUser user, int amountOfMessagesToDelete = 100)
        {
            if (user == Context.User)
                amountOfMessagesToDelete++; //Because it will count the purge command as a message

            var messages = await Context.Message.Channel.GetMessagesAsync(amountOfMessagesToDelete).FlattenAsync();

            var result = messages.Where(x => x.Author.Id == user.Id && x.CreatedAt >= DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(14)));

            await (Context.Message.Channel as SocketTextChannel).DeleteMessagesAsync(result);

        }
        [Command("clear", RunMode = RunMode.Async)]
        [Remarks("Clears An Amount Of Messages")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Clear(int amountOfMessagesToDelete)
        {
            await (Context.Message.Channel as SocketTextChannel).DeleteMessagesAsync(await Context.Message.Channel.GetMessagesAsync(amountOfMessagesToDelete + 1).FlattenAsync());
        }

        [RequireOwner]
        //[RequireRoleAttribute("Admin")]
        [Command("write")]
        [Summary("Echoes a message.")]
        public async Task Write(string msg, string ch = "0")
        {

            ulong uch = UInt64.Parse(ch);
            if (ch.Equals("0") || Context.Guild.GetChannel(uch) == null)
                await Context.Channel.SendMessageAsync(msg);
            else
            {
                var channel = Context.Guild.GetChannel(uch) as ISocketMessageChannel;
                await channel.SendMessageAsync(msg);
            }
        }

        [RequireOwner]
        //[RequireRoleAttribute("Admin")]
        [Command("writeEmbed")]
        public async Task writeEmbed(string title, string msg, byte R, byte G, byte B)
        {

            var embed = new EmbedBuilder();
            embed.WithTitle(title);
            embed.WithDescription(msg);
            embed.WithColor(new Color(R, G, B));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        [RequireOwner]
        //[RequireRoleAttribute("Admin")]
        [Command("writeEmbed")]
        public async Task writeEmbed(string title, string msg, string color)
        {
            byte R = 0, G = 0, B = 0;
            if (color.Equals("red"))
            {
                R = 255;
                G = 0;
                B = 0;
            }
            var embed = new EmbedBuilder();
            embed.WithTitle(title);
            embed.WithDescription(msg);
            embed.WithColor(new Color(R, G, B));
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
