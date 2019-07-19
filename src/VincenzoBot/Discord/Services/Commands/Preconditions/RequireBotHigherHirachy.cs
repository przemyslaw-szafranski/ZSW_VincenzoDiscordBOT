using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace VincenzoBot.Discord.Services.Commands.Preconditions
{
        public class RequireBotHigherHirachy : ParameterPreconditionAttribute
        {
            public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
            {
                var user = value is SocketGuildUser ? (SocketGuildUser)value : null;

                var bot = (context.Guild as SocketGuild).GetUser(context.Client.CurrentUser.Id);
                if ((user != null) && (bot.Hierarchy < user.Hierarchy))
                    return Task.FromResult(PreconditionResult.FromError($"User hirachy is lower than parameter user {user.Username}."));

                return Task.FromResult(PreconditionResult.FromSuccess());
            }
        }
}
