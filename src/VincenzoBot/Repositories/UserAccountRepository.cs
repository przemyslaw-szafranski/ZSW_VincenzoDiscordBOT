using System.Collections.Generic;
using System.IO;
using System.Linq;
using VincenzoBot;
using VincenzoBot.Storages;
using VincenzoDiscordBot.Models;

namespace VincenzoDiscordBot.Repositories
{
    //TODO ZAPIS KONKRETNEGO UZYTKOWNIKA
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
        public void CreateUserAccount(UserAccount user)
        {
            _logger.Log("Creating user: "+user.Username);
            var newAccount = new UserAccount()
            {
                Id = user.Id,
                Nickname = user.Username,
                //Joined = user.JoinedAt,
                Haczyks = Constants.WELCOME_HACZYKS,
                Xp = 0
            };
            SaveAccount(newAccount);
        }
        public void SaveAccounts()
        {
            _logger.Log("Saving all user accounts: ");
            string json;
            foreach (var a in _accounts)
            {
                json = JsonConvert.SerializeObject(a, Formatting.Indented);
                File.WriteAllText(Constants.USERACCOUNTS_FOLDER, json);
            }
        }
        public void SaveAccount(SocketUser socketUser)
        {
            _logger.Log("Saving user: " + socketUser.Username);
            UserAccount user = GetUser(socketUser);
            string json = JsonConvert.SerializeObject(user, Formatting.Indented);
            File.WriteAllText(Constants.USERACCOUNTS_FOLDER, json);
        }
        private void SaveAccount(UserAccount user)
        {
            _logger.Log("Saving user: " + user.Nickname);
            string json = JsonConvert.SerializeObject(user, Formatting.Indented);
            File.WriteAllText(Constants.USERACCOUNTS_FOLDER, json);
        }
        public UserAccount GetUser(SocketUser user)
        {
            var result = from a in _accounts
                         where a.Id == user.Id
                         select a;
            return result.FirstOrDefault();
        }
        /*   public string GetAccountFilePath(ulong id)
           {
               var filePath = Path.Combine(Path.Combine(_directoryPath, $"{id}.json"));
               return File.Exists(filePath) ? filePath : String.Empty;
           }*/
    }
}
