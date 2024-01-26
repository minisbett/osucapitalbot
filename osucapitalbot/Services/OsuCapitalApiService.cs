using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using osucapitalbot.Models.Capital;
using osucapitalbot.Utilities;
using System.Net;
using System.Text;

namespace osucapitalbot.Services;

/// <summary>
/// The osu!capital API service is responsible for communicating with osu!capital.
/// </summary>
public class OsuCapitalApiService
{
  private readonly HttpClient _http;
  private readonly CachingService _caching;
  private readonly ILogger<OsuCapitalApiService> _logger;

  public OsuCapitalApiService(IHttpClientFactory httpClientFactory, CachingService caching, ILogger<OsuCapitalApiService> logger)
  {
    _http = httpClientFactory.CreateClient("capitalapi");
    _caching = caching;
    _logger = logger;
  }

  /// <summary>
  /// Returns whether a connection to osu!capital can be established.
  /// </summary>
  /// <returns>Result whether a connection can be established.</returns>
  public async Task<Result> CheckAvailableAsync()
  {
    try
    {
      // Try to send a request to the base URL.
      await _http.GetAsync("");
      return Result.Success();
    }
    catch (Exception ex)
    {
      _logger.LogError("CheckAvailableAsync() failed: {Message}", ex.Message);
      return Error.Unspecific;
    }
  }

  /// <summary>
  /// Returns all trending stocks on osu!capital.
  /// </summary>
  /// <returns>The trending stocks.</returns>
  public async Task<Result<Stock[]>> GetTrendingStocksAsync()
  {
    try
    {
      // Get the raw JSON from the API.
      string json = await _http.GetStringAsync("trending_stocks");

      // Parse the JSON and return it.
      return JsonConvert.DeserializeObject<Stock[]>(json) ?? throw new Exception("The deserialized JSON is null.");
    }
    catch (Exception ex)
    {
      // If an error occured, log it and return null.
      _logger.LogError("An error occured while trying to get the trending stocks: {Message}", ex.Message);
      return Result.Failure<Stock[]>(Error.Unspecific);
    }
  }
}
