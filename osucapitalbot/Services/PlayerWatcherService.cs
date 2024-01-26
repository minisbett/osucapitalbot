using Discord.Addons.Hosting;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using osucapitalbot.Models.Osu;
using osucapitalbot.Utilities;

namespace osucapitalbot.Services;

/// <summary>
/// The player watcher service monitors all PP changes in the top #10,000 of osu! players on the global ranking and reports them on Discord.
/// </summary>
public class PlayerWatcherService : DiscordClientService
{
  private readonly OsuApiService _osuApi;
  private readonly IServiceProvider _provider;

  /// <summary>
  /// The amount of top players to scan. The maximum supported by the API is 10,000.
  /// </summary>
  private const int TOP_PLAYERS_TO_SCAN = 10_000;

  /// <summary>
  /// The next ranking page to scan. This number resets to 1 once all pages have been scanned.<br/>
  /// One pages equals to 50 players, meaning the maximum page is <see cref="TOP_PLAYERS_TO_SCAN"/> / 50.
  /// </summary>
  private int _page = 1;

  /// <summary>
  /// Bool whether a full page cycle has been performed yet, meaning the cache is fully initialized.
  /// </summary>
  private bool _cycledOnce = false;

  /// <summary>
  /// A cache of all known ranked osu! users, allowing the service to compare newly received values with their previous ones.
  /// </summary>
  private List<RankedOsuUser> _userCache = new List<RankedOsuUser>();

  public PlayerWatcherService(OsuApiService osuApi, IServiceProvider provider, DiscordSocketClient client, ILogger<PlayerWatcherService> logger)
    : base(client, logger)
  {
    _osuApi = osuApi;
    _provider = provider;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
#if DEBUG
    return;
#endif

    Logger.LogInformation("Service is running.");

    // Run the user watcher asynchronously.
    while (!stoppingToken.IsCancellationRequested)
    {
      // Get the users on the current page.
      Result<RankedOsuUser[]> result = await _osuApi.GetRankingAsync(_page);
      if (result.IsFailure)
      {
        // If an error occured, wait a moment and try again.
        Logger.LogError($"Failed to get the leaderboard ranking: {result.Error}");
        await Task.Delay(10_000);
        continue;
      }

      // Go through all users and handle them accordingly.
      foreach (RankedOsuUser user in result.Value)
        await HandleUserAsync(user);

      // Move to the next page or start over at 1 if the cycle completed. If a cycle completed for the first time, log it.
      _page = _page % (TOP_PLAYERS_TO_SCAN / 50) + 1;
      if(_page == 1 && !_cycledOnce)
      {
        _cycledOnce = true;
        Logger.LogInformation("Initial cache population performed.");
      }
    }
  }

  /// <summary>
  /// Handles the ranked osu! user received from the leaderboard ranking.
  /// </summary>
  /// <param name="user">The user.</param>
  private async Task HandleUserAsync(RankedOsuUser user)
  {
    // If the user is unknown to the service, cache it so on the next check it can be compared with the new values.
    RankedOsuUser? last = _userCache.FirstOrDefault(u => u.User.Id == user.User.Id);
    if (last is null)
    {
      _userCache.Add(user);
      return;
    }

    // Check whether the PP of the user changed.
    if (Math.Abs(user.PP - last.PP) >= 1.0)
      await Client
        .GetGuild(RuntimeConfiguration.OSUCAPITAL_GUILD_ID)
        .GetTextChannel(RuntimeConfiguration.OSUCAPITAL_STOCKWATCHER_CHANNEL)
        .SendMessageAsync(embed: Embeds.StockWatcherPPChange(user, last));

    // Replace the last user object with the new one in cache.
    _userCache.Remove(last);
    _userCache.Add(user);
  }
}