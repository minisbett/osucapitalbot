using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osucapitalbot.Models.Osu;

/// <summary>
/// Represents the ranking of an osu! user in a leaderboard.
/// </summary>
public class RankedOsuUser
{
  /// <summary>
  /// The performance points of the user in the ranking.
  /// </summary>
  [JsonProperty("pp")]
  public double PP { get; private set; }

  /// <summary>
  /// The global rank of the user in the ranking.
  /// </summary>
  [JsonProperty("global_rank")]
  public int Rank { get; private set; }

  /// <summary>
  /// The user.
  /// </summary>
  [JsonProperty("user")]
  public OsuUser User { get; private set; } = null!;
}
