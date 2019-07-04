using System;
using System.Threading.Tasks;
using VincenzoBot.Models;
using VincenzoBot.Repositories;

namespace VincenzoBot
{
    public class LevelingService
    {
        private readonly UserAccountRepository _userRepo;
        private readonly ILogger _logger;
        public LevelingService(UserAccountRepository userRepo, ILogger logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task RewardMessage(UserAccount user, string message)
        {
            if (user.LastMessage.AddSeconds(Constants.MESSAGE_REWARD_COOLDOWN) < DateTime.Now 
                && message.Length>Constants.MESSAGE_REWARD_MIN_LENGTH)
            {
                Random randObj = new Random();
                uint xp = (uint)randObj.Next((int)(1000/Constants.LEVELING_DIFFICULTY), (int)(2500 /Constants.LEVELING_DIFFICULTY));
                user.Xp += xp;
                user.LastMessage = DateTime.Now;
                _userRepo.SaveAccount(user);
                _logger.Log($"Giving user {user.Nickname} {xp} exp!");

            }
            //else
            //{
            //    _logger.Log($"{user.Nickname} wont recieve exp! {user.LastMessage.AddSeconds(Constants.MESSAGE_REWARD_COOLDOWN) - DateTime.Now}");
            //}
        }
        public async Task<bool> LevelUp(UserAccount user)
        {
            var expNeededToLevel = 5 * (user.Level ^ 2) + 50 * user.Level + 100;
            if (expNeededToLevel <= user.Xp)
            {
                user.Level++;
                user.Xp -= expNeededToLevel;
                _userRepo.SaveAccount(user);
                _logger.Log($"User {user.Nickname} leveled up to {user.Level} level!");
                return true;
            }
            return false;
            
        }
    }
}
