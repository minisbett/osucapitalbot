using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osucapitalbot.Models.Capital;

/// <summary>
/// Represents an osu!capital stock.
/// </summary>
public class Stock
{
  /// <summary>
  /// The ID of this stock. This always equals to the osu! user ID of the user this stock represents.
  /// </summary>
  [JsonProperty("stock_id")]
  public int StockId { get; private set; }

  /// <summary>
  /// The price for one share of this stock.
  /// </summary>
  [JsonProperty("share_price")]
  public double Price { get; private set; }

  /// <summary>
  /// The ranking of this stock. (# highest share price)
  /// </summary>
  [JsonProperty("share_rank")]
  public int Rank { get; private set; }

  /// <summary>
  /// The percentage on how much the share price of this stock changed on average over the last 48 hours.
  /// </summary>
  [JsonProperty("share_price_change_percentage")]
  public double AverageChangeLast48h { get; private set; }

  /// <summary>
  /// The name of the osu! user this stock belongs to.
  /// </summary>
  [JsonProperty("osu_name")]
  public string OsuName { get; private set; } = "";

  /// <summary>
  /// The avatar url of the osu! user this stock belongs to.
  /// </summary>
  [JsonProperty("osu_picture")]
  public string OsuAvatarUrl { get; private set; } = "";

  /// <summary>
  /// The rank of the osu! user this stock belongs to.
  /// </summary>
  [JsonProperty("osu_rank")]
  public int OsuRank { get; private set; }

  /// <summary>
  /// The PP of the osu! user this stock belongs to.
  /// </summary>
  [JsonProperty("osu_pp")]
  public double OsuPP { get; private set; }

  /// <summary>
  /// The datetime at which this stock was last updated.
  /// </summary>
  [JsonProperty("last_updated")]
  public DateTime UpdatedAt { get; private set; }
}
