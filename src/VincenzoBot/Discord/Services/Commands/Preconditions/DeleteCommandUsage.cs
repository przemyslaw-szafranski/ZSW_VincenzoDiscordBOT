using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VincenzoBot.Discord.Services.Commands.Preconditions
{
    class DeleteCommandUsage : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            context.Message.DeleteAsync();
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
