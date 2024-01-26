using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osucapitalbot;

/// <summary>
/// Contains various constants related to the configuration of the runtime. (e.g. Discord snowflakes)
/// </summary>
public static class RuntimeConfiguration
{
#if !DEBUG
  /// <summary>
  /// The Guild ID of the osu!capital Discord.
  /// </summary>
  public const ulong OSUCAPITAL_GUILD_ID = 1129202618053435542;

  /// <summary>
  /// The channel ID of the stock-watcher channel in the <see cref="OSUCAPITAL_GUILD_ID"/> Guild.
  /// </summary>
  public const ulong OSUCAPITAL_STOCKWATCHER_CHANNEL = 1199668177412567121;
#else
  /// <summary>
  /// The Guild ID of the osu!capital Discord.
  /// </summary>
  public const ulong OSUCAPITAL_GUILD_ID = 1009893337639161856;

  /// <summary>
  /// The channel ID of the player-watcher channel in the <see cref="OSUCAPITAL_GUILD_ID"/> Guild.
  /// </summary>
  public const ulong OSUCAPITAL_STOCKWATCHER_CHANNEL = 1200452032008556615;
#endif
}
