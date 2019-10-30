using System;
using System.Collections.Generic;
using System.Linq;
using VincenzoBot.Models;
using VincenzoBot.Repositories;

namespace VincenzoBot.Services.Discord
{
    class UserAccountService
    {
        private static UserAccountRepository _userRepo;
        public UserAccountService(UserAccountRepository userRepo)
        {
            _userRepo = userRepo;
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


    }
}
