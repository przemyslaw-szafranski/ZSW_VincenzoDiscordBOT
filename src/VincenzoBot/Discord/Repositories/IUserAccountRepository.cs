using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using VincenzoBot.Models;

namespace VincenzoBot.Repositories
{
    public interface IUserAccountRepository
    {
        List<UserAccount> GetAccounts();
        Task<UserAccount> CreateUserAccountAsync(SocketUser user);
        Task SaveAccounts();
        Task SaveAccount(SocketUser socketUser);
        Task SaveAccount(UserAccount user);
        Task<UserAccount> GetOrCreateUserAsync(SocketUser user);
        Task UpdateUserFileAndNicknameAsync(UserAccount user, string nickname);
        void UpdateUserFileAndNickname(SocketUser socketUser);
        UserAccount GetUserById(ulong id);
        void RemoveUser(ulong id);
    }
}