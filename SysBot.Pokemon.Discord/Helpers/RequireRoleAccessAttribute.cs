﻿using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord;

/// <summary>
/// Requires an assigned role in order to accept commands. Can be used by sudo users if satisfied.
/// </summary>
public sealed class RequireRoleAccessAttribute(string RoleName) : PreconditionAttribute
{
    // Create a field to store the specified name

    // Create a constructor so the name can be specified

    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        var mgr = SysCordSettings.Manager;
        if (mgr.Config.AllowGlobalSudo && mgr.CanUseSudo(context.User.Id))
            return Task.FromResult(PreconditionResult.FromSuccess());

        // Check if this user is a Guild User, which is the only context where roles exist
        if (context.User is not SocketGuildUser gUser)
            return Task.FromResult(PreconditionResult.FromError("You must be sending the message from a guild to run this command."));

        var roles = gUser.Roles;
        if (mgr.CanUseSudo(roles.Select(z => z.Name)))
            return Task.FromResult(PreconditionResult.FromSuccess());

        if (!mgr.GetHasRoleAccess(RoleName, roles.Select(z => z.Name)))
            return Task.FromResult(PreconditionResult.FromError("You do not have the required role to run this command."));

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}