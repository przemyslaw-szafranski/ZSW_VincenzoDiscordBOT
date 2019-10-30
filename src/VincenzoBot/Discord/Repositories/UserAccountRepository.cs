using Discord.WebSocket;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VincenzoBot;
using VincenzoBot.Storages;
using VincenzoBot.Discord;
using VincenzoBot.Models;

namespace VincenzoBot.Repositories
{
    public class UserAccountRepository
    {
        private List<UserAccount> _accounts = null;
        private readonly ILogger _logger;
        private readonly IDataStorage _storage;
        public UserAccountRepository(ILogger logger, IDataStorage storage)
        {
            _logger = logger;
            _storage = storage;
            _accounts = LoadOrCreate();
        }
        private List<UserAccount> LoadOrCreate()
        {
            List<UserAccount> list = new List<UserAccount>();
            if (Directory.Exists(Constants.USERACCOUNTS_FOLDER)
                && Directory.GetFiles(Constants.USERACCOUNTS_FOLDER).Count() > 0)
            {
                UserAccount newUser;
                foreach (var f in Directory.GetFiles(Constants.USERACCOUNTS_FOLDER).Select(Path.GetFileNameWithoutExtension))
                {
                    newUser = _storage.RestoreObject<UserAccount>(Constants.USERACCOUNTS_FOLDER + "/" + f);
                    list.Add(newUser);
                }
                _logger.Log("Loading users accounts");
                return list;
            }
            else //just create a directory 
            {
                _logger.Log("Users directory or users accounts in directory not found.");
                Directory.CreateDirectory(Constants.USERACCOUNTS_FOLDER);
                return new List<UserAccount>();
            }
        }
        public TimeSpan GiveDaily(SocketUser user)
        {
            UserAccount account = GetUserById(user.Id);
            var difference = account.LastDaily.AddDays(1).Subtract(DateTime.Now);
            if (difference.Ticks <= 0)
            {
                account.Haczyks += Constants.DAILY_HACZYKS_GAIN;
                account.LastDaily = DateTime.Now;
                SaveAccount(account);
                _logger.Log($"{account.Nickname} has recieved {Constants.DAILY_HACZYKS_GAIN} daily Haczyks.");
                return TimeSpan.Zero;
            }
            else
            {
                return difference;
            }
        }
        public void GiveHaczyks(SocketUser user, int haczyks)
        {
            UserAccount account = GetUserById(user.Id);
            if (haczyks > 0)
            {
                account.Haczyks += (uint)haczyks;
                _logger.Log($"{account.Nickname} has recieved {haczyks} Haczyks.");
            }
            else
            {
                account.Haczyks -= (uint)haczyks;
                _logger.Log($"{account.Nickname} has lost {haczyks} Haczyks.");
            }
            SaveAccount(account);
        }
        public List<UserAccount> GetAccounts()
        {
            return _accounts;
        }
        public async Task<UserAccount> CreateUserAccountAsync(SocketUser user)
        {
            _logger.Log("Creating user: " + user.Username);
            var newAccount = new UserAccount()
            {
                Id = user.Id,
                Nickname = user.Username
            };
            _accounts.Add(newAccount);
            await SaveAccount(newAccount);
            return newAccount;
        }
        public async Task SaveAccounts()
        {
            _logger.Log("Saving all user accounts: ");
            foreach (var a in _accounts)
            {
                string filePath = Constants.USERACCOUNTS_FOLDER + $"/{a.Nickname}";
                _storage.StoreObject(a, filePath);
            }
        }
        public async Task SaveAccount(SocketUser socketUser)
        {
            _logger.Log("Saving user: " + socketUser.Username);
            UserAccount user = await GetOrCreateUserAsync(socketUser);
             _storage.StoreObject(user, Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}");
        }
        public async Task SaveAccount(UserAccount user)
        {
            _logger.Log("Saving user: " + user.Nickname);
           _storage.StoreObject(user, Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}");
        }
        public async Task<UserAccount> GetOrCreateUserAsync(SocketUser user)
        {
            UserAccount result = _accounts.Where(i => i.Id==user.Id).FirstOrDefault();
            var guildUser = user as SocketGuildUser;
            if (result.Nickname != guildUser.Nickname && guildUser.Nickname!=null)
                await UpdateUserFileAndNicknameAsync(result,guildUser.Nickname);
            if (result == null)
            {
                var newUser = CreateUserAccountAsync(user);
                return newUser.Result;
            }
            return result;
        }

        public async Task UpdateUserFileAndNicknameAsync(UserAccount user, string nickname)
        {
            _storage.UpdateObject(user, Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}", Constants.USERACCOUNTS_FOLDER + $"/{nickname}");
            _logger.Log($"Changing file an nickname of user: {user.Nickname} to {nickname}");
            user.Nickname = nickname;
            await SaveAccount(user);
        }

        public UserAccount GetUserById(ulong id)
        {
            foreach (UserAccount a in _accounts)
            {
                if (a.Id == id) return a;
            }
            return null;
        }
        public void RemoveUser(ulong id)
        {
            var user = GetUserById(id);
            _accounts.Remove(user);
            _storage.DeleteObject(Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}");
            if (!_storage.Exists(Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}"))
                _logger.Log("Deleted user: " + user.Nickname);
        }
        public static implicit operator UserAccountRepository(Mock<UserAccountRepository> v)
        {
            return v;
        }
        /*   public string GetAccountFilePath(ulong id)
  {
      var filePath = Path.Combine(Path.Combine(_directoryPath, $"{id}.json"));
      return File.Exists(filePath) ? filePath : String.Empty;
  }*/
    }
}
