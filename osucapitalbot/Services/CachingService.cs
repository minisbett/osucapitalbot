using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osucapitalbot.Services;

/// <summary>
/// The caching service handles all kind of caching of values in both memory and database.
/// </summary>
public class CachingService
{
  private readonly PersistenceService _persistence;
  private readonly ILogger<CachingService> _logger;

  public CachingService(PersistenceService persistence, ILogger<CachingService> logger)
  {
    _persistence = persistence;
    _logger = logger;
  }
}
