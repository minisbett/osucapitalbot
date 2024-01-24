using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using osucapitalbot.Models.Capital;
using osucapitalbot.Services;
using osucapitalbot.Utilities;
using System.Diagnostics;

namespace osucapitalbot.Modules;

/// <summary>
/// A wrapper around the interaction module base for all modules.
/// </summary>
public class ModuleBase : InteractionModuleBase<SocketInteractionContext>
{
  private readonly OsuCapitalApiService _capital;
  private readonly OsuApiService _osu;
  private readonly PersistenceService _persistence;

  public ModuleBase(OsuCapitalApiService capital = null!, OsuApiService osu = null!, PersistenceService persistence = null!)
  {
    _capital = capital;
    _osu = osu;
    _persistence = persistence;
  }

  /// <summary>
  /// Returns the trending stocks on osu!capital. If an error occured, null is returned.
  /// </summary>
  /// <returns>The trending stocks, or null if an error occured.</returns>
  public async Task<Stock[]?> GetTrendingSocksAsync()
  {
    // Get the data from the osu!capital API. If the API returned an error, notify this user accordingly.
    Stock[]? stocks = await _capital.GetTrendingStocksAsync();
    if (stocks is null)
      await FollowupAsync(embed: Embeds.InternalError("An error occured while trying to get the trending stocks."));

    // Return the stocks as-is.
    return stocks;
  }
}