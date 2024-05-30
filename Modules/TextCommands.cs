using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TextCommandFramework.Services;

namespace TextCommandFramework.Modules;

// Modules must be public and inherit from an IModuleBase
public class TextCommands : ModuleBase<SocketCommandContext>
{
    // Dependency Injection will fill this value in for us
    public PictureService PictureService { get; set; }

    public async Task SendCatPicture(Stream stream)
    {
        // Get a stream containing an image of a cat
        stream.Seek(0, SeekOrigin.Begin);
        // Streams must be seeked to their beginning before being uploaded!
        await Context.Channel.SendFileAsync(stream, "cat.png");
    }

    [Command("ping")]
    [Alias("pong", "hello")]
    public Task PingAsync() => ReplyAsync("pong!");

    [Command("help")]
    public async Task HelpAsync()
    {
        List<string> list = new();
        using StreamReader reader = new(Directory.GetCurrentDirectory() + "/help.txt");
        string text = reader.ReadToEnd();
        await ReplyAsync(text.Replace('!', Program._config["commandchard"][0]));
    }

    [Command("cat")]
    public async Task CatAsync()
    {
        await SendCatPicture(await PictureService.GetCatPictureAsync());
    }

    [Command("catsay")]
    public async Task CatSayAsync([Remainder] string message)
    {
        await SendCatPicture(await PictureService.GetCatSayPictureAsync(message));
    }

    [Command("catlike")]
    public async Task CatTagAsync([Remainder] string message)
    {
        await SendCatPicture(await PictureService.GetCatTagPictureAsync(message));
    }

    [Command("catlikesay")]
    public async Task CatTagSayAsync([Remainder] string message)
    {
        await SendCatPicture(await PictureService.GetCatTagSayPictureAsync(message));
    }

    [Command("catgif")]
    public async Task CatGifAsync()
    {
        await SendCatPicture(await PictureService.GetCatGifAsync());
    }

    [Command("catgifsay")]
    public async Task CatGifSayAsync([Remainder] string message)
    {
        await SendCatPicture(await PictureService.GetCatSayPictureAsync(message));
    }

    // Get info on a user, or the user who invoked the command if one is not specified
    [Command("userinfo")]
    public async Task UserInfoAsync(IUser user = null)
    {
        user ??= Context.User;

        await ReplyAsync(user.ToString());
    }

    [Command("ban")]
    [RequireContext(ContextType.Guild)]
    // make sure the user invoking this command can ban
    [RequireUserPermission(GuildPermission.BanMembers)]
    // make sure the bot itself can ban
    [RequireBotPermission(GuildPermission.BanMembers)]
    public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
    {
        await user.SendMessageAsync(
            $"You have been banned from {Context.Guild.Name} for {reason}."
        );
        await Context.Guild.AddBanAsync(user, 0, reason);
        await ReplyAsync($"Banned {user.Username}#{user.Discriminator}");
    }

    [Command("kick")]
    [RequireContext(ContextType.Guild)]
    // make sure the user invoking this command can ban
    [RequireUserPermission(GuildPermission.KickMembers)]
    // make sure the bot itself can ban
    [RequireBotPermission(GuildPermission.KickMembers)]
    public async Task KickUserAsync(IGuildUser user, [Remainder] string reason = null)
    {
        await user.SendMessageAsync(
            $"You have been kicked from {Context.Guild.Name} for {reason}."
        );
        await Context.Guild.RemoveBanAsync(user);
        await ReplyAsync($"Kicked {user.Username}#{user.Discriminator}");
    }
}
