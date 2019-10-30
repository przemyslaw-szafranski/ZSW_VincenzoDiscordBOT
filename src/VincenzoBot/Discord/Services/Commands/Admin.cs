using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VincenzoBot.Config;
using VincenzoBot.Discord.Services.Commands.Preconditions;
using VincenzoBot.Preconditions;
using VincenzoBot.Repositories;
using VincenzoBot.Services.Discord;

namespace VincenzoBot.Modules
{
    //Commands which are about user information
    public class Admin : ModuleBase<SocketCommandContext>
    {
        private readonly UserAccountRepository _userRepo;
        private readonly BotConfigRepository _configRepo;
        private readonly ILogger _logger;
        private static readonly OverwritePermissions denyOverwrite = new OverwritePermissions(addReactions: PermValue.Deny, sendMessages: PermValue.Deny, attachFiles: PermValue.Deny);
        public Admin(UserAccountRepository userRepo, BotConfigRepository config, ILogger logger)
        {
            _userRepo = userRepo;
            _configRepo = config;
            _logger = logger;
        }
        [Command("saveAccounts"), Alias("saccounts", "savea")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SaveAccounts()
        {
            await Context.Message.DeleteAsync();
            await _userRepo.SaveAccounts();
            await Context.Channel.SendMessageAsync("*Vincenzo zamyka kartotekę i chowa do szuflady*\nZapisałem ich dane... Którego odstrzelimy?");
        }
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
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetGame([Remainder] string gamename)
        {
            await Context.Client.SetGameAsync(gamename);
            await SendLogToChannel($"{Context.User.Username} changed game to `{gamename}`");
        }

        #region Ordnung
        [Command("ban")]
        [Remarks("Ban A User")]
        [DeleteCommandUsage]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban([NoSelf][RequireBotHigherHirachy] IGuildUser user, [Remainder] string reason=null)
        {
            await SendLogToChannel($"{Context.User.Username} has banned user {user.Mention} `{reason}`");
            _logger.Log($"{Context.User.Username} has banned user {user.Username + user.Id}{reason}");
            await Context.Guild.AddBanAsync(user);
        }

        [Command("kick")]
        [Remarks("Kicks user from the server")]
        [DeleteCommandUsage]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick([NoSelf][RequireBotHigherHirachy] IGuildUser user, [Remainder]string reason = null)
        {
            if (user.IsBot || user.IsWebhook) return;
            await user.SendMessageAsync($"Zostałeś wyrzucony z serwera **{Context.Guild.Name}**, powód: `{reason}`\nUważaj, bo następnym razem osobiście Cię sprzątnę...");
            await user.SendMessageAsync("http://media1.giphy.com/media/Pu3MzS917wZHi/giphy.gif");
            await SendLogToChannel($"{Context.User.Username} has kicked user {user.Mention} `{reason}`");
            _logger.Log($"{Context.User.Username} has kicked user {user.Username + user.Id}{reason}");
            await user.KickAsync(reason);
        }
        [Command("mute")]
        [DeleteCommandUsage]
        [Remarks("Mutes A User")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        public async Task Mute([NoSelf][RequireBotHigherHirachy] IGuildUser user, [Remainder]string reason=null)
        {
            await SendLogToChannel($"{Context.User.Username} has muted user {user.Mention} `{reason}`");
            _logger.Log($"{Context.User.Username} has muted user {user.Username + user.Id} {reason}");
            await Context.Channel.SendMessageAsync($"*Vincenzo zakneblował buzię {user.Mention}*, powód: `{reason}`");
            //await user.SendMessageAsync($"*Vincenzo zakneblował Ci buzię*, powód: `{reason}`\nhttps://d.wattpad.com/story_parts/663262466/images/156dd45d110b244a348680547166.gif");
            var muteRole = await GetMuteRole(user.Guild);
            if (!user.RoleIds.Contains(muteRole.Id))
                await user.AddRoleAsync(muteRole).ConfigureAwait(false);
        }

        [Command("unmute")]
        [DeleteCommandUsage]
        [Remarks("Unmutes A User")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        public async Task Unmute([NoSelf] IGuildUser user)
        {
            await SendLogToChannel($"{Context.User.Username} has unmuted user {user.Mention}");
            _logger.Log($"{Context.User.Username} has unmuted user {user.Username + user.Id}");
            await Context.Channel.SendMessageAsync($"*Vincenzo zerwał knebel z buzi {user.Mention}*\nEhh... Szkoda, dopiero się rozkręcałem!");
            try { await user.RemoveRoleAsync(await GetMuteRole(user.Guild)).ConfigureAwait(false); } catch { }
        }
        [Command("purge")]
        [DeleteCommandUsage]
        [Remarks("Purges A User's Last Messages. Default Amount To Purge Is 100")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(SocketGuildUser user, int amountOfMessagesToDelete = 100)
        {
            if (user == Context.User)
                amountOfMessagesToDelete++; //Because it will count the purge command as a message

            var messages = await Context.Message.Channel.GetMessagesAsync(amountOfMessagesToDelete).FlattenAsync();

            var result = messages.Where(x => x.Author.Id == user.Id && x.CreatedAt >= DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(14)));

            await (Context.Message.Channel as SocketTextChannel).DeleteMessagesAsync(result);

        }

        [Command("clear", RunMode = RunMode.Async)]
        [DeleteCommandUsage]
        [Remarks("Clears An Amount Of Messages")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Clear(int amountOfMessagesToDelete)
        {
            await (Context.Message.Channel as SocketTextChannel).DeleteMessagesAsync(await Context.Message.Channel.GetMessagesAsync(amountOfMessagesToDelete + 1).FlattenAsync());
        }
        #endregion Ordnung

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
        public async Task SendLogToChannel(string msg)
        {
            var channel = Context.Guild.GetChannel(_configRepo._config.CommandsOutputChannelID) as ISocketMessageChannel;
            await channel.SendMessageAsync(msg);
        }
        public async Task<IRole> GetMuteRole(IGuild guild)
        {
            const string defaultMuteRoleName = "Zakneblowany";

            var muteRoleName = "Zakneblowany";

            var muteRole = guild.Roles.FirstOrDefault(r => r.Name == muteRoleName);

            if (muteRole == null)
            {
                try
                {
                    muteRole = await guild.CreateRoleAsync(muteRoleName, GuildPermissions.None).ConfigureAwait(false);
                }
                catch
                {
                    muteRole = guild.Roles.FirstOrDefault(r => r.Name == muteRoleName) ?? await guild.CreateRoleAsync(defaultMuteRoleName, GuildPermissions.None).ConfigureAwait(false);
                }
            }

            foreach (var toOverwrite in (await guild.GetTextChannelsAsync()))
            {
                try
                {
                    if (!toOverwrite.PermissionOverwrites.Any(x => x.TargetId == muteRole.Id && x.TargetType == PermissionTarget.Role))
                    {
                        await toOverwrite.AddPermissionOverwriteAsync(muteRole, denyOverwrite)
                                .ConfigureAwait(false);
                        await Task.Delay(200).ConfigureAwait(false);
                    }
                }
                catch { }
            }
            return muteRole;
        }
    }
   

}
