using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Kitcheer.API.Data;

namespace Kitcheer.API.Controllers;

/// <summary>
/// Adatbázis állapot és kapcsolat ellenőrzését biztosító API végpontok
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DatabaseController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseController> _logger;

    public DatabaseController(ApplicationDbContext context, IConfiguration configuration, ILogger<DatabaseController> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Adatbázis kapcsolat tesztelése
    /// </summary>
    /// <returns>Kapcsolat állapota</returns>
    /// <response code="200">Sikeres kapcsolat</response>
    /// <response code="400">Kapcsolat hiba</response>
    /// <response code="500">Szerver hiba</response>
    [HttpGet("test")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            if (canConnect)
            {
                return Ok(new { success = true, message = "Sikeresen kapcsolódott az adatbázishoz" });
            }
            else
            {
                return BadRequest(new { success = false, message = "Nem sikerült kapcsolódni az adatbázishoz" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hiba az adatbázis kapcsolat tesztelésekor");
            return StatusCode(500, new { success = false, message = "Hiba történt", error = ex.Message });
        }
    }

    /// <summary>
    /// Részletes adatbázis információk lekérése
    /// </summary>
    /// <returns>Adatbázis információk (verzió, méret, táblák, migrációk)</returns>
    /// <response code="200">Sikeres lekérés</response>
    /// <response code="500">Szerver hiba</response>
    [HttpGet("info")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetDatabaseInfo()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var builder = new NpgsqlConnectionStringBuilder(connectionString);

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var versionCommand = new NpgsqlCommand("SELECT version()", connection);
            var version = await versionCommand.ExecuteScalarAsync();

            var dbSizeCommand = new NpgsqlCommand($"SELECT pg_size_pretty(pg_database_size('{builder.Database}'))", connection);
            var dbSize = await dbSizeCommand.ExecuteScalarAsync();

            // Get applied and pending migrations
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

            // Get table information
            var tablesCommand = new NpgsqlCommand(@"
                SELECT table_name, 
                (SELECT COUNT(*) FROM information_schema.columns WHERE table_name = t.table_name AND table_schema = 'public') as column_count
                FROM information_schema.tables t
                WHERE table_schema = 'public' AND table_type = 'BASE TABLE'
                ORDER BY table_name", connection);

            var tables = new List<object>();
            using var reader = await tablesCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tables.Add(new
                {
                    name = reader.GetString(0),
                    columnCount = reader.GetInt32(1)
                });
            }

            return Ok(new
            {
                success = true,
                database = builder.Database,
                host = builder.Host,
                port = builder.Port,
                version = version?.ToString(),
                size = dbSize?.ToString(),
                tables = tables,
                appliedMigrations = appliedMigrations.ToList(),
                pendingMigrations = pendingMigrations.ToList(),
                migrationsUpToDate = !pendingMigrations.Any()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hiba az adatbázis információk lekérésekor");
            return StatusCode(500, new { success = false, message = "Hiba történt", error = ex.Message });
        }
    }
}