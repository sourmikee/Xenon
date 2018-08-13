﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using Xenon.Core;

#endregion

namespace Xenon.Services.External
{
    public static class UtilService
    {
        public static Regex CodeBlockRegex;

        public static string GetName(this CommandInfo command)
        {
            return !string.IsNullOrWhiteSpace(command.Module.Group)
                ? $"{(string.IsNullOrWhiteSpace(command.Module.Name) ? null : $"{command.Module.Name} ")}{command.Name}"
                : $"{command.Name}";
        }

        public static string GetUsage(this IEnumerable<CommandInfo> commands, ShardedCommandContext context)
        {
            var overloadUsages = new List<string>();
            foreach (var command in commands)
            {
                var usage = command.GetName();
                foreach (var parameter in command.Parameters)
                    if (parameter.IsOptional)
                        usage += $" ({parameter.Name})";
                    else
                        usage += $" <{parameter.Name}>";
                overloadUsages.Add(usage);
            }

            return string.Join("\n", overloadUsages);
        }

        public static string GetUsage(this CommandInfo command, ShardedCommandContext context)
        {
            var usage = command.GetName();
            foreach (var parameter in command.Parameters)
                if (parameter.IsOptional)
                    usage += $" ({parameter.Name})";
                else
                    usage += $" <{parameter.Name}>";
            return usage;
        }

        public static string GetUsage(this ModuleInfo module, ShardedCommandContext context)
        {
            var overloadUsages = new List<string>();
            foreach (var command in module.Commands)
            {
                var usage = command.GetName();
                foreach (var parameter in command.Parameters)
                    if (parameter.IsOptional)
                        usage += $" ({parameter.Name})";
                    else
                        usage += $" <{parameter.Name}>";
                overloadUsages.Add(usage);
            }

            return string.Join("\n", overloadUsages);
        }


        public static string ToHastebin(this string code, HttpClient http = null)
        {
            CodeBlockRegex = CodeBlockRegex ?? new Regex(PublicVariables.CodeBlockRegex,
                                 RegexOptions.Compiled | RegexOptions.Multiline);
            http = http ?? new HttpClient();
            var codes = CodeBlockRegex.Matches(code);

            var data = JObject.Parse(http
                .PostAsync("https://hastebin.com/documents",
                    new StringContent(string.Join("\n\n\n", codes.Select(x => x.Groups[2])))).GetAwaiter().GetResult()
                .Content.ReadAsStringAsync().GetAwaiter().GetResult());
            return $"https://hastebin.com/{data["key"]}";
        }

        public static EmbedBuilder NormalizeEmbed(this EmbedBuilder embed,
            ColorType colorType, Random random, bool withRequested = false, ShardedCommandContext context = null)
        {
            if (withRequested && context != null)
                embed.WithFooter(
                    $"Requested by {(context.User as IGuildUser)?.Nickname ?? context.User.Username}#{context.User.Discriminator}",
                    context.User.GetAvatarUrl() ?? context.User.GetDefaultAvatarUrl());
            embed.SetColor(colorType, random)
                .WithColor(PublicVariables.DefaultColor);
            if (withRequested)
                embed.WithFooter(
                    $"Requested by {(context.User as IGuildUser)?.Nickname ?? context.User.Username}#{context.User.Discriminator}",
                    context.User.GetAvatarUrl() ?? context.User.GetDefaultAvatarUrl());
            return embed;
        }

        public static EmbedBuilder NormalizeEmbed(string title, string description, ColorType colorType, Random random,
            bool withRequested = false, ShardedCommandContext context = null)
        {
            var embed = new EmbedBuilder();
            if (withRequested && context != null)
                embed.WithFooter(
                    $"Requested by {(context.User as IGuildUser)?.Nickname ?? context.User.Username}#{context.User.Discriminator}",
                    context.User.GetAvatarUrl() ?? context.User.GetDefaultAvatarUrl());
            embed.SetColor(colorType, random)
                .WithTitle(title)
                .WithDescription(description);
            if (withRequested)
                embed.WithFooter(
                    $"Requested by {(context.User as IGuildUser)?.Nickname ?? context.User.Username}#{context.User.Discriminator}",
                    context.User.GetAvatarUrl() ?? context.User.GetDefaultAvatarUrl());
            return embed;
        }

        private static EmbedBuilder SetColor(this EmbedBuilder embed, ColorType colorType, Random random = null)
        {
            random = random ?? new Random();
            switch (colorType)
            {
                case ColorType.Random:
                    embed.WithColor(new Color(random.Next(255), random.Next(255), random.Next(255)));
                    break;
                case ColorType.Normal:
                    embed.WithColor(PublicVariables.DefaultColor);
                    break;
            }

            return embed;
        }

        public static bool GetSetting(this Server server, ServerSettings settings)
        {
            return !server.Settings.TryGetValue(settings, out var state) || state;
        }

        public static bool GetChannelSettings(this Server server, ulong channelId, ServerSettings settings)
        {
            server.ChannelSettings.TryGetValue(channelId, out var channelSettings);
            var serverState = server.GetSetting(settings);
            if (channelSettings != null && channelSettings.TryGetValue(settings, out var state)) return state;

            return serverState;
        }

        public static string ToMessage(this string message, IUser user, IGuild guild)
        {
            var dictionary = new Dictionary<string, string>
            {
                {"%mention%", user.Mention},
                {"%user%", (user as IGuildUser)?.Nickname ?? user.Username},
                {"%server%", guild.Name}
            };

            foreach (var pair in dictionary)
                message = message.Replace(pair.Key, pair.Value, StringComparison.OrdinalIgnoreCase);

            return message;
        }
    }
}