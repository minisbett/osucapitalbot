using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace osucapitalbot.Services;

/// <summary>
/// The osu! API service is responsible for communicating with the osu! API v2.
/// </summary>
public class OsuApiService
{
  private readonly HttpClient _http;
  private readonly ILogger<OsuApiService> _logger;

  /// <summary>
  /// The client ID for the osu! API v2.
  /// </summary>
  private readonly string _clientId;

  /// <summary>
  /// The client secret for the osu! API v2.
  /// </summary>
  private readonly string _clientSecret;

  /// <summary>
  /// The date when the osu! API v2 access token expires.
  /// </summary>
  private DateTimeOffset _accessTokenExpiresAt = DateTimeOffset.MinValue;

  public OsuApiService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<OsuApiService> logger)
  {
    _http = httpClientFactory.CreateClient("osuapi");
    _clientId = config["OSU_OAUTH_CLIENT_ID"] ?? throw new InvalidOperationException("The environment variable 'OSU_OAUTH_CLIENT_ID' is not set.");
    _clientSecret = config["OSU_OAUTH_CLIENT_SECRET"] ?? throw new InvalidOperationException("The environment variable 'OSU_OAUTH_CLIENT_SECRET' is not set.");
    _logger = logger;
  }

  /// <summary>
  /// Returns a bool whether a connection to the osu! API v2 can be established.
  /// </summary>
  /// <returns>Bool whether a connection can be established.</returns>
  public async Task<bool> IsAvailableAsync()
  {
    // Make sure a valid access token exists.
    if (!await EnsureAccessTokenAsync())
      return false;

    try
    {
      // Try to send a request to the base URL of the osu! API v2.
      HttpResponseMessage response = await _http.GetAsync("api/v2");

      // Check whether it returns the expected result.
      if (response.StatusCode != HttpStatusCode.NotFound)
        throw new Exception($"API returned status code {response.StatusCode}. Expected: NotFound (404)."); // Gives 401 Unauthorized if invalid client credentials.

      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError("IsAvailable() returned false: {Message}", ex.Message);
      return false;
    }
  }

  /// <summary>
  /// Ensures that the current osu! API v2 access token is valid and returns whether it is valid or was successfully refreshed.
  /// </summary>
  /// <returns>Bool whether the access token is valid or was successfully refreshed.</returns>
  public async Task<bool> EnsureAccessTokenAsync()
  {
    // Check whether the access token is still valid.
    if (DateTimeOffset.Now < _accessTokenExpiresAt)
      return true;

    _logger.LogInformation("The osu! API v2 access token has expired. Requesting a new one...");

    try
    {
      // Send the request.
      HttpResponseMessage response = await _http.PostAsync($"oauth/token",
        new FormUrlEncodedContent(new Dictionary<string, string>()
        {
        { "client_id", _clientId },
        { "client_secret", _clientSecret },
        { "grant_type", "client_credentials"},
        { "scope", "public" }
        }));

      // Parse the response object into a dynamic object.
      var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

      // Check whether the response was successful.
      if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.InternalServerError) // For some reason invalid client id = internal server error.
        throw new Exception("Unauthorized.");
      if (result?.access_token is null)
        throw new Exception("The oauth access token response did not contain an access token.");

      // Set the new access token and expiration date and return true.
      _http.DefaultRequestHeaders.Remove("Authorization");
      _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.access_token}");
      _accessTokenExpiresAt = DateTimeOffset.Now.AddSeconds((int)result.expires_in - 10);
      _logger.LogInformation("The osu! API v2 access token has been updated and expires at {date}.", _accessTokenExpiresAt);
    }
    catch (Exception ex)
    {
      _logger.LogError("Failed to request an osu! API v2 access token: {Message}", ex.Message);
      return false;
    }

    return true;
  }
}