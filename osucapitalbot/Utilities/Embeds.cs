using Discord;
using osucapitalbot.Models.Capital;
using osucapitalbot.Models.Osu;
using System.Runtime.Serialization;

namespace osucapitalbot.Utilities;

/// <summary>
/// Provides embeds for the application.
/// </summary>
internal static class Embeds
{
  /// <summary>
  /// Returns a base embed all other embeds should be based on.
  /// </summary>
  private static EmbedBuilder BaseEmbed => new EmbedBuilder()
    .WithColor(new Color(0x99377A))
    .WithFooter($"osu!capital bot v{Program.VERSION} by minisbett", "https://osucapital.com/favicon.ico")
    .WithCurrentTimestamp();

  /// <summary>
  /// Returns an error embed for displaying an internal error message.
  /// </summary>
  /// <param name="message">The error message.</param>
  public static Embed InternalError(string message) => BaseEmbed
    .WithColor(Color.DarkRed)
    .WithTitle("An internal error occured.")
    .WithDescription(message)
    .Build();

  /// <summary>
  /// Returns an error embed for displaying an error message.
  /// </summary>
  /// <param name="message">The error message.</param>
  public static Embed Error(string message) => BaseEmbed
    .WithColor(Color.Red)
    .WithDescription(message)
    .Build();

  /// <summary>
  /// Returns an error embed for displaying a neutral message.
  /// </summary>
  /// <param name="message">The neutral message.</param>
  public static Embed Neutral(string message) => BaseEmbed
    .WithDescription(message)
    .Build();

  /// <summary>
  /// Returns an error embed for displaying a success message.
  /// </summary>
  /// <param name="message">The success message.</param>
  public static Embed Success(string message) => BaseEmbed
    .WithColor(Color.Green)
    .WithDescription(message)
    .Build();

  /// <summary>
  /// Returns an embed for displaying info about the bot (version, uptime, api status, ...).
  /// </summary>
  /// <param name="osuAvailable">Bool whether the osu! API v1 is available.</param>
  /// <param name="capitalAvailable">Bool whether osu!capital is available.</param>
  public static Embed Info(bool osuAvailable, bool capitalAvailable) => BaseEmbed
    .WithTitle($"Information about osu!capital bot {Program.VERSION}")
    .WithDescription("This bot aims to provide interaction with [osu!capital](https://osucapital.com/) via Discord." +
                     " If any issues come up, please ping `@minisbett` or send them a DM.")
    .AddField("Uptime", $"{(DateTime.UtcNow - Program.STARTUP_TIME).ToUptimeString()}\n\n[Source](https://github.com/minisbett/osucapitalbot)", true)
    .AddField("API Status", $"osu!api v2 {new Emoji(osuAvailable ? "✅" : "❌")}\nosu!capital {new Emoji(capitalAvailable ? "✅" : "❌")}\n", true)
    .WithThumbnailUrl("https://cdn.discordapp.com/attachments/1009893434087198720/1198990312157220964/favicon.png")
    .Build();

  /// <summary>
  /// Returns an embed for displaying the change of PP of an osu! user.
  /// </summary>
  /// <param name="user">The current state of the user.</param>
  /// <param name="last">The last state of the user.</param>
  public static Embed StockWatcherPPChange(RankedOsuUser user, RankedOsuUser last)
  {
    // Calculate statistics.
    double ppDiff = user.PP - last.PP;
    double ppDiffPercent = (user.PP / last.PP - 1) * 100;
    int rankDiff = user.Rank - last.Rank;
    double rankDiffPercent = (last.Rank * 1d / user.Rank - 1) * 100;
    
    return BaseEmbed
      .WithColor(user.PP > last.PP ? Color.Green : Color.Red)
      .WithAuthor(user.User.Name, user.User.AvatarUrl, $"https://osucapital.com/stock/{user.User.Id}")
      .WithDescription($"[View Profile](https://osu.ppy.sh/u/{user.User.Id}) • [View Stock](https://osucapital.com/stock/{user.User.Id})")
      .AddField($"{last.PP:N0}pp → {user.PP:N0}pp", $"{ppDiff:+0;-0;0}pp ({ppDiffPercent:+0.00;-0.00}%)", true)
      .AddField($"#{last.Rank:N0} → #{user.Rank:N0}", $"{(rankDiff <= 0 ? "+" : "-")}#{Math.Abs(rankDiff)} ({rankDiffPercent:+0.00;-0.00}%)", true)
      .Build();
    }

  /// <summary>
  /// Returns an embed for displaying the currently trending stocks.
  /// </summary>
  /// <param name="stocks">The trending stocks.</param>
  public static Embed TrendingStocks(Stock[] stocks)
  {
    // Build the stock listing.
    List<string> description = new List<string>();
    foreach(Stock stock in stocks)
      description.Add($"**+{stock.AverageChangeLast48h:N2}% [{stock.OsuName}](https://osucapital.com/stock/{stock.Id})** • " +
                      $"[osu!](https://osu.ppy.sh/u/{stock.Id}) • *{stock.Price:N2}* {_emojis["osucoin"]} • #{stock.OsuRank:N0}");

    return BaseEmbed
      .WithColor(new Color(0x61D6D6))
      .WithTitle("Currently Trending Stocks")
      .WithDescription(string.Join("\n", description))
      .Build();
  }

  /// <summary>
  /// Returns an embed containing login instructions.
  /// </summary>
  public static Embed LoginInstructions => BaseEmbed
    .WithTitle("Login Instructions")
    .WithDescription("In order to allow the bot to interact with osu!capital on your behalf, you will need to provide your osu!capital session cookie. Instructions on how to obtain it can be found below.")
    .Build();

  /// <summary>
  /// A dictionary containing quick access to custom emojis.
  /// </summary>
  private static Dictionary<string, string> _emojis = new Dictionary<string, string>()
  {
    { "osucoin", "<:osucoin:1200223778043600910>" }
  };
}

