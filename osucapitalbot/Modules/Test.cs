#if DEBUG
using Discord.Interactions;
using osucapitalbot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osucapitalbot.Modules;

public class Test : ModuleBase
{
  public Test(OsuCapitalApiService capital) : base(capital) { }

  [SlashCommand("test", "Command for testing.")]
  public async Task HandleAsync()
  {
    await DeferAsync();

    var x = await GetTrendingSocksAsync();
    ;
  }
}
#endif