﻿#region

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;
using Xenon.Core;
using Xenon.Services.Nsfw;

#endregion

namespace Xenon.Modules
{
    [CheckNsfw]
    [CommandCategory(CommandCategory.Nsfw)]
    public class NsfwModule : CommandBase
    {
        private readonly HttpClient _httpClient;
        private readonly NsfwService _nsfwService;
        private readonly Random _random;

        public NsfwModule(HttpClient httpClient, Random random, NsfwService nsfwService)
        {
            _httpClient = httpClient;
            _random = random;
            _nsfwService = nsfwService;
        }

        [Command("ass")]
        [Summary("Sends some nice ass :)")]
        public async Task AssAsync()
        {
            var random = _random.Next(6012);
            var data = JArray.Parse(await _httpClient.GetStringAsync($"http://api.obutts.ru/butts/{random}")).First;
            var embed = new EmbedBuilder()
                .WithImageUrl($"http://media.obutts.ru/{data["preview"]}");

            await ReplyEmbedAsync(embed, ColorType.Normal, true);
        }

        [Command("boobs")]
        [Summary("Sends some nice boobs ;)")]
        public async Task BoobsAsync()
        {
            var random = _random.Next(12965);
            var data = JArray.Parse(await _httpClient.GetStringAsync($"http://api.oboobs.ru/boobs/{random}")).First;
            var embed = new EmbedBuilder()
                .WithImageUrl($"http://media.oboobs.ru/{data["preview"]}");

            await ReplyEmbedAsync(embed, ColorType.Normal, true);
        }

        [Command("hentai")]
        [Summary("Sends a random hentai picture :D")]
        public async Task HentaiAsync()
        {
            await _nsfwService.SendImageFromCategory(Context, "hentai");
        }

        [Command("nude")]
        [Summary("Sends some nice nudes :P")]
        public async Task NudeAsync()
        {
            await _nsfwService.SendImageFromCategory(Context, "4k");
        }

        [Command("nudegif")]
        [Summary("Sends a nice nude gif d:")]
        public async Task NudeGifAsync()
        {
            await _nsfwService.SendImageFromCategory(Context, "pgif");
        }

        [Command("anal")]
        [Summary("Sends a carrot in a melon imao")]
        public async Task AnalAsync()
        {
            await _nsfwService.SendImageFromCategory(Context, "anal");
        }

        [Command("pussy")]
        [Summary("Sends a melon with a hole")]
        public async Task PussyAsync()
        {
            await _nsfwService.SendImageFromCategory(Context, "pussy");
        }
    }
}