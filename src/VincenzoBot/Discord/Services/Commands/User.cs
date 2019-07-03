using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VincenzoBot.Models;
using VincenzoBot.Preconditions;
using VincenzoBot.Repositories;
using VincenzoBot.Services.Discord;

namespace VicenzoBot.Modules
{
    //Commands which are about user information
    public class User : ModuleBase<SocketCommandContext>
    {
        //private UserAccountRepository _userRepo;
        //public User(UserAccountRepository userRepo)
        //{
        //    _userRepo=userRepo;
        //}
        [Command("mycard")]
        [Cooldown(10)]
        public async Task MyStats()
        {
            UserAccount user = _userRepo.GetUser(Context.User);
            var embed = new EmbedBuilder();
            embed.WithTitle("");
            embed.WithColor(0xFF, 0xFF, 0x80);//TODO kolorek zalezny od rangi
            embed.WithAuthor("👤 " + Context.User.Username + " - statystyki");//TODO emoji od rangi
            embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            embed.AddField("Nazwa:", "#404", true);
            embed.AddField("YouTube:", user.Yt_id, true);
            embed.AddField("Data dołączenia:", Context.Guild.CurrentUser.JoinedAt, true);
            embed.AddField("Poziom:", "#404", true);
            embed.AddField("EXP:", user.Xp, true);
            embed.AddField("Haczyki ֏", user.Haczyks, true);
            embed.WithFooter("Piękną wizytówkę sponsoruje Hantick", "https://cdn.discordapp.com/emojis/590579218463588363.png?v=1");
            await Context.Channel.SendMessageAsync("*Vincenzo wyciąga z teczki wizytówkę:*", false, embed.Build());
        }
        [Command("updateYouTube"), Alias("updateYT", "upYT")]
        [Cooldown(20, true)]
        public async Task addYouTubeAccount(string ytName)
        {
            UserAccount user = _userRepo.GetUser(Context.User);
            if (ytName == null || ytName == "" || ytName.Length <= 2 || ytName.Length > 20)
            {
                await Context.User.SendMessageAsync($"Coś Ci się chyba z tym kontem YouTube pomyliło (**{ytName}**) :face_palm:");
                return;
            }
            else if (ytName.Equals(user.Yt_id))
            {
                await Context.User.SendMessageAsync($"Masz już taką nazwę konta YouTube (**{ytName}**)(*^mycard*) :unamused:");
            }
            // else if(UserAccountService.isYTNameTaken(ytName))
            // {
            //     await Context.User.SendMessageAsync($"Ktoś już ma taką nazwę konta YouTube (**{ytName}**)(*^mycard*) :thinking:");
            // }
            else
            {
                user.Yt_id = ytName;
                _userRepo.SaveAccounts();
                string msg = "*Vincenzo wypisuje coś na Twojej wizytówce...* \n" +
                    Utilities.GetPhrase($"YTNameUpdate{Utilities.Random(0, 4)}") +
                    $"\n\nKonto o nazwie: **{user.Yt_id}** - "
                    + Utilities.GetPhrase("YTNameUpdateINFO");
                await Context.User.SendMessageAsync(msg);
            }
        }
    }

}
