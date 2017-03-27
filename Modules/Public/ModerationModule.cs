﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Ro
{
	public class ModerationModule : ModuleBase
	{
        //fields
		public ModuleInfo _mi;
		public DiscordSocketClient _cl;
        //constructor
		public ModerationModule(CommandService serv, DiscordSocketClient client)
		{
			ModuleInfo mi = serv.Modules.First();
			DiscordSocketClient cl = client;
			_mi = mi;
			_cl = cl;
		}
        //commands
		[Command("mute"), Summary("Mutes a user"), RequireUserPermission(GuildPermission.ManageMessages)]
		    public async Task Mute([Remainder] string a)
		{
            GuildPermissions perms = GuildPermissions.None;
            perms = Context.Guild.EveryoneRole.Permissions.Modify(sendMessages:false);
			var mutedUserId = Context.Message.MentionedUserIds.First();
			var guild = Context.Guild;
			IGuildUser user = await guild.GetUserAsync(mutedUserId);
            Dictionary<string, ulong> roledict = guild.Roles.ToDictionary(x => { return x.Name; }, y => { return y.Id; });
            IRole muteRole = null;
            if (roledict.ContainsKey("Muted") == false)
            {
                muteRole = await guild.CreateRoleAsync("Muted", perms);
            }
            else {
                ulong id = 1;
                roledict.TryGetValue("Muted", out id);
                muteRole = Context.Guild.GetRole(id);
            }
                await user.AddRoleAsync(muteRole);
            await ReplyAsync($"Muted {user}!");
		}

		[Command("unmute"), Summary("Unmutes a user"), RequireUserPermission(GuildPermission.ManageMessages)]
            public async Task Unmute([Remainder] string a)
		{
            var mutedUserId = Context.Message.MentionedUserIds.First();
			var guild = Context.Guild;
            Dictionary<string, ulong> roledict = guild.Roles.ToDictionary(x => { return x.Name; }, y => { return y.Id; });
            IGuildUser user = await guild.GetUserAsync(mutedUserId);
            ulong id = 1;
            roledict.TryGetValue("Muted", out id);
			var muteRole = guild.GetRole(id);

			await user.RemoveRoleAsync(muteRole);
            await ReplyAsync($"Unmuted {user}!");
        }

        [Command("clear"), Summary("Clears messages."), RequireUserPermission(GuildPermission.ManageMessages)]
            public async Task ClearMessages([Remainder] int b)
        {
            await Context.Channel.DeleteMessagesAsync(await Context.Channel.GetMessagesAsync(b + 1).Flatten());
            await ReplyAsync($"{Context.Message.Author.Mention}, successfully cleared {b} messages!");
        }



    }
}