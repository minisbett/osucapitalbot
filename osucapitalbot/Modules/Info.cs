using Discord.Interactions;
using osucapitalbot.Services;
using osucapitalbot.Utilities;

namespace osucapitalbot.Modules;

/// <summary>
/// The interaction module for the info command, displaying general info about the bot.
/// </summary>
public class InfoCommandModule : InteractionModuleBase<SocketInteractionContext>
{
  private readonly OsuApiService _osu;
  private readonly OsuCapitalApiService _capital;

  public InfoCommandModule(OsuApiService osu, OsuCapitalApiService capital)
  {
    _osu = osu;
    _capital = capital;
  }

  [SlashCommand("info", "Displays info about the bot.")]
  public async Task HandleAsync()
  {
    await DeferAsync();

    // Return the info embed to the user.
    await FollowupAsync(embed: Embeds.Info((await _osu.CheckAvailableAsync()).IsSuccessful, (await _capital.CheckAvailableAsync()).IsSuccessful));
  }
}
