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
using VincenzoBot.Models;
using VincenzoBot.Preconditions;
using VincenzoBot.Repositories;
using VincenzoBot.Services.Discord;

namespace VincenzoBot.Modules
{
    public class Economy : ModuleBase<SocketCommandContext>
    {
        private readonly UserAccountRepository _userRepo;
        private readonly BotConfigRepository _configRepo;
        public Economy(UserAccountRepository userRepo, BotConfigRepository config)
        {
            _userRepo = userRepo;
            _configRepo = config;
        }
        [Command("daily")]
        [Remarks("Gives user a daily amount of Haczyks")]
        [Cooldown(60)]
        public async Task GiveDaily()
        {
            await Context.Message.DeleteAsync();
            var time = _userRepo.GiveDaily(Context.User);
            if (time == TimeSpan.Zero)
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Username} odebrał dzienną nagrodę ({Constants.DAILY_HACZYKS_GAIN} haczyków).");
            }
            else
                await Context.User.SendMessageAsync($"Gdzie z tymi łapskami! Do odebrania dziennej nagrody pozostało Ci {time.ToString(@"hh\:mm")}");
        }
        //HAZARD
        //TODO obrażanie jak ma sie za malo kasy :D
        //TODO improve
        //TODO embed?
        [Cooldown(10)]
        [Command("coin", RunMode = RunMode.Async)]
        [DeleteCommandUsage]
        [Remarks("Obstawiasz wynik podrzucenia monety, możesz wygrać lub przegrać.")]
        public async Task Coin(int bet, [Remainder]string side)
        {
            side = side.ToLower();
            if (bet <= 0 || (!side.Equals("orzel") && !side.Equals("orzeł") && !side.Equals("reszka")) || _userRepo.GetUserById(Context.User.Id).Haczyks<bet) return;
            if (bet > Constants.COIN_MAX_BET)
                await Context.User.SendMessageAsync($"Maksymalna stawka podrzucania monetą to {Constants.COIN_MAX_BET}");
            else
            {
                Random rnd = new Random();
                int value = rnd.Next(0, 2);
                string betstring = $"**{Context.User.Username} obstawił:** {side} za {bet} w rzucie monetą.";
                var message = await Context.Channel.SendMessageAsync(betstring);
                await Task.Delay(1000);
                await message.ModifyAsync(msg => msg.Content = betstring+"\n *Vincenzo podrzuca monetą...*");
                await Task.Delay(2000);
                if (value == 0)
                {
                    betstring = betstring + "\n*Vincenzo wyrzucił orła*";
                    await message.ModifyAsync(msg => msg.Content = betstring);
                    if ((side.Equals("orzel") || side.Equals("orzeł")))
                    {
                        _userRepo.GiveHaczyks(Context.User, bet);
                        betstring = betstring + $" 💰😄 +{bet}֏";
                        await message.ModifyAsync(msg => msg.Content = betstring);
                    }
                    else
                    {
                        _userRepo.GiveHaczyks(Context.User, -bet);
                        betstring = betstring + $" 💸😪 -{bet}֏";
                        await message.ModifyAsync(msg => msg.Content = betstring);
                    }
                }
                else if (value == 1)
                {
                    betstring = betstring + "\n*Vincenzo wyrzucił reszkę*";
                    await message.ModifyAsync(msg => msg.Content = betstring);
                    if (side.Equals("reszka"))
                    {
                        _userRepo.GiveHaczyks(Context.User, bet);
                        betstring = betstring + $" 💰😄 +{bet}֏";
                        await message.ModifyAsync(msg => msg.Content = betstring);
                    }
                    else
                    {
                        _userRepo.GiveHaczyks(Context.User, -bet);
                        betstring = betstring + $" 💸😪 -{bet}֏";
                        await message.ModifyAsync(msg => msg.Content = betstring);
                    }
                }
            }
        }
    }
}
