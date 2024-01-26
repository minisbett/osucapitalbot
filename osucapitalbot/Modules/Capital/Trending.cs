using Discord.Interactions;
using osucapitalbot.Models.Capital;
using osucapitalbot.Services;
using osucapitalbot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace osucapitalbot.Modules.Capital;

/// <summary>
/// The module for the "/trending" command, displaying all trending stocks.
/// </summary>
public class TrendingModule : ModuleBase
{
  public TrendingModule(OsuCapitalApiService capital) : base(capital) { }

  [SlashCommand("trending", "Displays the currently trending stocks.")]
  public async Task HandleAsync()
  {
    await DeferAsync();

    // Get the currently trending stocks.
    Result<Stock[]> stocks = await GetTrendingSocksAsync();
    if (stocks.IsFailure)
      return;

    // Display the embed to the user.
    await FollowupAsync(embed: Embeds.TrendingStocks(stocks.Value));
  }
}
