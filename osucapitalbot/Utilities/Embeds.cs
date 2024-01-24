using Discord;

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
    .WithColor(new Color(0x99377A /* 0xF1C40F */))
    .WithFooter($"osu!capital bot v{Program.VERSION} by minisbett", "https://osucapital.com/favicon.ico")
    .WithCurrentTimestamp();

  /// <summary>
  /// Returns an error embed for displaying an internal error message.
  /// </summary>
  /// <param name="message">The error message.</param>
  /// <returns>An embed for displaying an internal error message.</returns>
  public static Embed InternalError(string message) => BaseEmbed
    .WithColor(Color.DarkRed)
    .WithTitle("An internal error occured.")
    .WithDescription(message)
    .Build();

  /// <summary>
  /// Returns an error embed for displaying an error message.
  /// </summary>
  /// <param name="message">The error message.</param>
  /// <returns>An embed for displaying an error message.</returns>
  public static Embed Error(string message) => BaseEmbed
    .WithColor(Color.Red)
    .WithDescription(message)
    .Build();

  /// <summary>
  /// Returns an error embed for displaying a neutral message.
  /// </summary>
  /// <param name="message">The neutral message.</param>
  /// <returns>An embed for displaying a neutral message.</returns>
  public static Embed Neutral(string message) => BaseEmbed
    .WithDescription(message)
    .Build();

  /// <summary>
  /// Returns an error embed for displaying a success message.
  /// </summary>
  /// <param name="message">The success message.</param>
  /// <returns>An embed for displaying a success message.</returns>
  public static Embed Success(string message) => BaseEmbed
    .WithColor(Color.Green)
    .WithDescription(message)
    .Build();

  /// <summary>
  /// Returns an embed for displaying info about the bot (version, uptime, api status, ...).
  /// </summary>
  /// <param name="osuAvailable">Bool whether the osu! API v1 is available.</param>
  /// <param name="capitalAvailable">Bool whether osu!capital is available.</param>
  /// <returns>An embed for displaying info about the bot.</returns>
  public static Embed Info(bool osuAvailable, bool capitalAvailable) => BaseEmbed
    .WithTitle($"Information about osu!capital bot {Program.VERSION}")
    .WithDescription("This bot aims to provide interaction with [osu!capital](https://osucapital.com/) via Discord." +
                     " If any issues come up, please ping `@minisbett` or send them a DM.")
    .AddField("Uptime", $"{(DateTime.UtcNow - Program.STARTUP_TIME).ToUptimeString()}\n\n[Source](https://github.com/minisbett/osucapitalbot)", true)
    .AddField("API Status", $"osu!api v2 {new Emoji(osuAvailable ? "✅" : "❌")}\nosu!capital {new Emoji(capitalAvailable ? "✅" : "❌")}\n", true)
    .WithThumbnailUrl("https://cdn.discordapp.com/attachments/1009893434087198720/1198990312157220964/favicon.png")
    .Build();

  public static Embed LoginInstructions => BaseEmbed
    .WithTitle("Login Instructions")
    .WithDescription("In order to allow the bot to interact with osu!capital on your behalf, you will need to provide your osu!capital session cookie. Instructions on how to obtain it can be found below.")
    .Build();
}