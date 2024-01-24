using Discord;
using Discord.Interactions;
using osucapitalbot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osucapitalbot.Modules.Capital;

/// <summary>
/// The module for the "login" command, allowing the user to log in using the osu!capital cookie.
/// </summary>
public class LoginModule : ModuleBase
{
  /*[SlashCommand("login", "Log into osu!capital on the bot.")]
  public async Task HandleCommandAsync()
  {
    await DeferAsync();

    // Build a button component which will open the login modal.
    ComponentBuilder builder = new ComponentBuilder()
      .WithButton("Login via Cookie", "login_cookie_button", emote: new Emoji("\uD83D\uDD10"));

    // Inform the user on how to get the login cookie, and add a button to open the login modal.
    await FollowupAsync(embed: Embeds.LoginInstructions, components: builder.Build());
  } */

  [ComponentInteraction("login_cookie_button")]
  public async Task HandleButtonAsync()
  {
    // Respond with the modal.
    await RespondWithModalAsync<CookieModal>("login_cookie_modal");
  }

  [ModalInteraction("login_cookie_modal")]
  public async Task HandleModalAsync(CookieModal modal)
  {
    await RespondAsync(modal.Cookie);
  }
}

public class CookieModal : IModal
{
  public string Title => "osu!capital Cookie Login";

  [ModalTextInput("cookie", TextInputStyle.Paragraph, "Paste your cookie here...", 100, 200)]
  public string Cookie { get; init; } = "";
}