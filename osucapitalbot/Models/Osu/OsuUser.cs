using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osucapitalbot.Models.Osu;

/// <summary>
/// Represents a user on the osu! API v2.
/// </summary>
public class OsuUser
{
  /// <summary>
  /// The ID of this user.
  /// </summary>
  [JsonProperty("id")]
  public int Id { get; private set; }

  /// <summary>
  /// The username of this user.
  /// </summary>
  [JsonProperty("username")]
  public string Name { get; private set; } = "";

  /// <summary>
  /// Bool whether the user is currently online or not. Will be false if the user hides their online statues.
  /// </summary>
  [JsonProperty("is_online")]
  public bool IsOnline { get; private set; }

  /// <summary>
  /// The URL to the avatar of thsi user.
  /// </summary>
  [JsonProperty("avatar_url")]
  public string AvatarUrl { get; private set; } = "";
}
