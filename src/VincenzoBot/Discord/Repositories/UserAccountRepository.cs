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
        private readonly DiscordLogger _logger;
        private readonly IDataStorage _storage;
        public UserAccountRepository(DiscordLogger logger, IDataStorage storage)
        {
            _logger = logger;
            _storage = storage;
            _accounts = LoadOrCreate();
        }
        public List<UserAccount> LoadOrCreate()
        {
            List<UserAccount> list = new List<UserAccount>();
            if (Directory.Exists(Constants.USERACCOUNTS_FOLDER) 
                && Directory.GetFiles(Constants.USERACCOUNTS_FOLDER).Count()>0)
            {
                string json;
                UserAccount newUser;
                foreach (var f in Directory.GetFiles(Constants.USERACCOUNTS_FOLDER))
                {
                    json = File.ReadAllText(f);
                    newUser = _storage.RestoreObject<UserAccount>(f);
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
        public void CreateUserAccount(SocketUser user)
        {
            _logger.Log("Creating user: "+user.Username);
            var newAccount = new UserAccount()
            {
                Id = user.Id,
                Nickname = user.Username
            };
            _accounts.Add(newAccount);
            SaveAccount(newAccount);
        }
        public void SaveAccounts()
        {
            _logger.Log("Saving all user accounts: ");
            foreach (var a in _accounts)
            {
                string filePath = Constants.USERACCOUNTS_FOLDER + $"/{a.Nickname}";
                _storage.StoreObject(a, filePath);
            }
        }
        public void SaveAccount(SocketUser socketUser)
        {
            _logger.Log("Saving user: " + socketUser.Username);
            UserAccount user = GetUser(socketUser);
            _storage.StoreObject(user, Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}");
        }
        private void SaveAccount(UserAccount user)
        {
            _logger.Log("Saving user: " + user.Nickname);
            _storage.StoreObject(user, Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}");
        }
        public UserAccount GetUser(SocketUser user)
        {
            var result = from a in _accounts
                         where a.Id == user.Id
                         select a;
            return result.FirstOrDefault();
        }
        public UserAccount GetUserById(ulong id)
        {
            foreach(UserAccount a in _accounts)
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
            if(!_storage.Exists(Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}"))
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
