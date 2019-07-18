using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VincenzoBot.Models;
using VincenzoBot.Repositories;

namespace VincenzoBot.Services.Discord
{
    public class UserAccountService
    {
        private static IUserAccountRepository _userRepo;
        public UserAccountService(IUserAccountRepository userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<TimeSpan> GiveDailyAsync(SocketUser socketUser)
        {
            UserAccount account = _userRepo.GetUserById(socketUser.Id);
            var difference = account.LastDaily.AddDays(1).Subtract(DateTime.Now);
            if (difference.Ticks <= 0)
            {
                account.Haczyks += Constants.DAILY_HACZYKS_GAIN;
                account.LastDaily = DateTime.Now;
                await _userRepo.SaveAccount(account);
                Log.Information($"{account.Nickname} has recieved {Constants.DAILY_HACZYKS_GAIN} daily Haczyks.");
                return TimeSpan.Zero;
            }
            else
            {
                return difference;
            }
        }
        public virtual async Task<TimeSpan> GiveDailyAsync(UserAccount account)
        {
            var difference = account.LastDaily.AddDays(1).Subtract(DateTime.Now);
            if (difference.Ticks <= 0)
            {
                account.Haczyks += Constants.DAILY_HACZYKS_GAIN;
                account.LastDaily = DateTime.Now;
                await _userRepo.SaveAccount(account);
                Log.Information($"{account.Nickname} has recieved {Constants.DAILY_HACZYKS_GAIN} daily Haczyks.");
                return TimeSpan.Zero;
            }
            else
            {
                return difference;
            }
        }
        public async Task GiveHaczyksAsync(SocketUser user, int haczyks)
        {
            UserAccount account = _userRepo.GetUserById(user.Id);
            if (haczyks > 0)
            {
                account.Haczyks += (uint)haczyks;
                Log.Information($"{account.Nickname} has recieved {haczyks} Haczyks.");
            }
            else
            {
                account.Haczyks -= (uint)haczyks;
                Log.Information($"{account.Nickname} has lost {haczyks} Haczyks.");
            }
            await _userRepo.SaveAccount(account);
        }
        public uint GetUserHaczyks(SocketUser user)
        {
            return _userRepo.GetUserById(user.Id).Haczyks;
        }
        public static bool IsYTNameTaken(string ytName)
        {
            List<UserAccount> accountList = _userRepo.GetAccounts();
            foreach(UserAccount a in accountList)
            {
                if (a.Yt_id.Equals(ytName)) return true;
            }
            return false;
        }
        public static string GetUsername(SocketUser user)
        {
            var guilduser = user as SocketGuildUser;
            if (guilduser.Nickname != null)
                return guilduser.Nickname;
            else
                return user.Username;
        }


    }
}
