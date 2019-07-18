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
using Serilog;

namespace VincenzoBot.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
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
            Log.Information("Loading users accounts");
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
                Log.Information("Loaded user accounts");
                return list;
            }
            else //just create a directory 
            {
                Log.Information("Users directory or users accounts in directory not found, creating.");
                Directory.CreateDirectory(Constants.USERACCOUNTS_FOLDER);
                return new List<UserAccount>();
            }
        }
        public List<UserAccount> GetAccounts()
        {
            return _accounts;
        }
        public async Task<UserAccount> CreateUserAccountAsync(SocketUser user)
        {
            Log.Information("Creating user: " + user.Username);
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
            Log.Information("Saving all user accounts: ");
            foreach (var a in _accounts)
            {
                string filePath = Constants.USERACCOUNTS_FOLDER + $"/{a.Nickname}";
                _storage.StoreObject(a, filePath);
            }
            Log.Information("Saved all user accounts");
        }
        public async Task SaveAccount(SocketUser socketUser)
        {
            UserAccount user = await GetOrCreateUserAsync(socketUser);
            Log.Information("Saving user: " + socketUser.Username);
             _storage.StoreObject(user, Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}");
        }
        public async Task SaveAccount(UserAccount user)
        {
            Log.Information("Saving user: " + user.Nickname);
           _storage.StoreObject(user, Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}");
        }
        public async Task<UserAccount> GetOrCreateUserAsync(SocketUser user)
        {
            UserAccount result = _accounts.Where(i => i.Id==user.Id).FirstOrDefault();
            var guildUser = user as SocketGuildUser;
            if (result == null)
            {
                var newUser = CreateUserAccountAsync(user);
                return newUser.Result;
            }
            if(guildUser.Nickname == null && !_storage.Exists(Constants.USERACCOUNTS_FOLDER + $"/{user.Username}"))
                await UpdateUserFileAndNicknameAsync(result, user.Username);
            else if (guildUser.Nickname!=null && user.Username != guildUser.Nickname && user.Id == guildUser.Id
                && !_storage.Exists(Constants.USERACCOUNTS_FOLDER + $"/{guildUser.Nickname}"))
                    await UpdateUserFileAndNicknameAsync(result, guildUser.Nickname);
            return result;
        }

        public async Task UpdateUserFileAndNicknameAsync(UserAccount user, string nickname)
        {

            Log.Information($"Changing file an nickname of user: {user.Nickname} to {nickname}");
            _storage.UpdateObject(user, Constants.USERACCOUNTS_FOLDER + $"/{user.Nickname}", Constants.USERACCOUNTS_FOLDER + $"/{nickname}");
            user.Nickname = nickname;
            await SaveAccount(user);
        }
        public void UpdateUserFileAndNickname(SocketUser socketUser)
        {
            _ = GetOrCreateUserAsync(socketUser).Result;
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
                Log.Warning("Deleted user: " + user.Nickname);
        }


        /*   public string GetAccountFilePath(ulong id)
{
var filePath = Path.Combine(Path.Combine(_directoryPath, $"{id}.json"));
return File.Exists(filePath) ? filePath : String.Empty;
}*/
    }
}
