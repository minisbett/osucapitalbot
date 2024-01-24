using osucapitalbot.Persistence;
using osucapitalbot.Utilities;
using Microsoft.EntityFrameworkCore;

namespace osucapitalbot.Services;

/// <summary>
/// The persistence service is responsible for managing the access to the persistence database.
/// </summary>
public class PersistenceService
{
  private readonly Database _database;

  public PersistenceService(Database database)
  {
    _database = database;
  }
}
