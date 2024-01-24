using Microsoft.EntityFrameworkCore;

namespace osucapitalbot.Persistence;

/// <summary>
/// The database context for the SQL persistence in the application.
/// </summary>
public class Database : DbContext
{
  public Database(DbContextOptions<Database> options) : base(options) { }
}