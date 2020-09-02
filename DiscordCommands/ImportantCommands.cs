using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModBot.DiscordCommands
{
    [RequireRoles(RoleCheckMode.Any, "Owner", "Co-Owner", "Administrator")]
    public class ImportantCommands : BaseCommandModule
    {

        public const ulong AnnouncementChannel = 721309081775898706;
        public const ulong ModerationLogsChannel = 738663328808697886;
        public const ulong MutedRole = 750204686631567432;
        public readonly DiscordEmbedBuilder.EmbedFooter EmbedFooter = new DiscordEmbedBuilder.EmbedFooter { Text = "Offering you the best mods in style." };

        [Command("Announcement")]
        public async Task Announcement(CommandContext context)
        {
            try
            {
                var Message = context.Message.Content.Replace(context.Message.Content.Substring(0, (context.Command.Name.Length + 1)), string.Empty).Trim();
                var Embed = new DiscordEmbedBuilder
                {
                    Title = "Announcement",
                    Color = DiscordColor.HotPink,
                    Description = Message,
                    Footer = EmbedFooter,
                    Url = "https://retrorecoveries.com/"
                };
                await context.Guild.GetChannel(AnnouncementChannel).SendMessageAsync(embed: Embed);
                var MentionEveryone = await context.Guild.GetChannel(AnnouncementChannel).SendMessageAsync("@everyone");
                await MentionEveryone.DeleteAsync();
            }
            catch
            {
                //
            }
        }

        [Command("Mute")]
        public async Task MuteUser(CommandContext context)
        {
            var Members = context.Message.MentionedUsers;
            if (Members.Count == 0)
            {
                await context.Channel.SendMessageAsync("No users found");
                return;
            }
            var MemberIds = string.Empty;
            foreach (var memb in Members)
            {
                MemberIds = MemberIds + $" <@{memb.Id}>";
            }

            var Message = string.Empty;

            try
            {
                Message = context.Message.Content.Replace(context.Message.Content.Substring(0, (context.Command.Name.Length + 3 + MemberIds.Length)), string.Empty).Trim();
                if (String.IsNullOrWhiteSpace(Message))
                {
                    Message = "No reason given";
                }
            }
            catch
            {
                Message = "No reason given";
            }

            var Embed = new DiscordEmbedBuilder
            {
                Title = $"Moderation",
                Description = $"Type: Mute \n Reason: {Message} \n Muted: {MemberIds} \n Muted By: {context.Message.Author.Mention}",
                Footer = EmbedFooter,
                Color = DiscordColor.Orange
            };

            await context.Guild.Members.Single(m => m.Key == context.Message.MentionedUsers.First().Id).Value.GrantRoleAsync(context.Guild.GetRole(MutedRole), Message); //Muted RoleId

            await context.Client.GetChannelAsync(ModerationLogsChannel).Result.SendMessageAsync(embed: Embed);
        }

        [Command("Unmute")]
        public async Task UnmuteUser(CommandContext context)
        {
            try
            {
                var Members = context.Message.MentionedUsers;
                if (Members.Count == 0)
                {
                    await context.Channel.SendMessageAsync("No users found");
                    return;
                }

                if (!context.Guild.Members.Where(m => m.Value == Members.First()).SingleOrDefault().Value.Roles.Contains(context.Guild.GetRole(MutedRole)))
                {
                    await context.Channel.SendMessageAsync("User is not muted");
                    return;
                }

                var MemberIds = string.Empty;
                foreach (var memb in Members)
                {
                    MemberIds = MemberIds + $" <@{memb.Id}>";
                }

                var Message = string.Empty;

                try
                {
                    Message = context.Message.Content.Replace(context.Message.Content.Substring(0, (context.Command.Name.Length + 3 + MemberIds.Length)), string.Empty).Trim();
                    if (String.IsNullOrWhiteSpace(Message))
                    {
                        Message = "No reason given";
                    }
                }
                catch
                {
                    Message = "No reason given";
                }

                var Embed = new DiscordEmbedBuilder
                {
                    Title = $"Moderation",
                    Description = $"Type: Unmute \n Reason: {Message} \n Unmuted: {MemberIds} \n Unmuted By: {context.Message.Author.Mention}",
                    Footer = EmbedFooter,
                    Color = DiscordColor.Orange
                };

                await context.Guild.Members.Single(m => m.Key == context.Message.MentionedUsers.First().Id).Value.RevokeRoleAsync(context.Guild.GetRole(750204686631567432), Message); //RoleId

                await context.Client.GetChannelAsync(ModerationLogsChannel).Result.SendMessageAsync(embed: Embed);
            }
            catch
            {
                return;
            }
        }


        [Command("Kick")]
        public async Task KickUser(CommandContext context)
        {
            var Members = context.Message.MentionedUsers;
            if (Members.Count == 0)
            {
                await context.Channel.SendMessageAsync("No users found");
                return;
            }
            var MemberIds = "";
            foreach (var memb in Members)
            {
                MemberIds = MemberIds + $" <@{memb.Id}>";
            }

            var Message = string.Empty;

            try
            {
                Message = context.Message.Content.Replace(context.Message.Content.Substring(0, (context.Command.Name.Length + 3 + MemberIds.Length)), string.Empty).Trim();
                if (String.IsNullOrWhiteSpace(Message))
                {
                    Message = "No reason given";
                }
            }
            catch
            {
                Message = "No reason given";
            }

            var Embed = new DiscordEmbedBuilder
            {
                Title = $"Moderation",
                Description = $"Type: Kick \n Reason: {Message} \n Kicked: {MemberIds} \n Kicked By: {context.Message.Author.Mention}",
                Footer = EmbedFooter,
                Color = DiscordColor.Orange
            };

            foreach (var member in Members)
            {
                var Id = member.Id;
                if (context.Member.Id == Id)
                {
                    await context.Channel.SendMessageAsync("You can't kick yourself!");
                    return;
                }
                await context.Guild.Members.SingleOrDefault(m => m.Key == Id).Value.RemoveAsync(Message);

            }

            await context.Client.GetChannelAsync(ModerationLogsChannel).Result.SendMessageAsync(embed: Embed);
        }

        [Command("Ban")]
        public async Task BanUser(CommandContext context)
        {
            var Members = context.Message.MentionedUsers;
            if (Members.Count == 0)
            {
                await context.Channel.SendMessageAsync("No users found");
                return;
            }
            var MemberIds = "";
            foreach (var memb in Members)
            {
                MemberIds = MemberIds + $" <@{memb.Id}>";
            }

            var Message = string.Empty;

            try
            {
                Message = context.Message.Content.Replace(context.Message.Content.Substring(0, (context.Command.Name.Length + 3 + MemberIds.Length)), string.Empty).Trim();
                if (String.IsNullOrWhiteSpace(Message))
                {
                    Message = "No reason given";
                }
            }
            catch
            {
                Message = "No reason given";
            }

            var Embed = new DiscordEmbedBuilder
            {
                Title = $"Moderation",
                Description = $"Type: Ban \n Reason: {Message} \n Banned: {MemberIds} \n Banned By: {context.Message.Author.Mention}",
                Footer = EmbedFooter,
                Color = DiscordColor.Orange
            };

            foreach (var member in Members)
            {
                var Id = member.Id;
                if (context.Member.Id == Id)
                {
                    await context.Channel.SendMessageAsync("You can't kick yourself!");
                    return;
                }

                await context.Guild.Members.SingleOrDefault(m => m.Key == Id).Value.BanAsync(7, Message);

            }

            await context.Client.GetChannelAsync(ModerationLogsChannel).Result.SendMessageAsync(embed: Embed);
        }

    }

}
