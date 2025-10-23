using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Kitcheer.API.Data;

namespace Kitcheer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpGet("test")]
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

    [HttpPost("create")]
    public async Task<IActionResult> CreateDatabase()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest(new { success = false, message = "Kapcsolati string nem található" });
            }

            // Parse connection string to get server connection without database
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;
            builder.Database = "postgres"; // Connect to default postgres database
            var serverConnectionString = builder.ToString();

            using var serverConnection = new NpgsqlConnection(serverConnectionString);
            await serverConnection.OpenAsync();

            // Check if database exists
            var checkDbCommand = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'", serverConnection);
            var exists = await checkDbCommand.ExecuteScalarAsync();

            if (exists != null)
            {
                return Ok(new { success = true, message = $"Az adatbázis '{databaseName}' már létezik" });
            }

            // Create database
            var createDbCommand = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", serverConnection);
            await createDbCommand.ExecuteNonQueryAsync();

            return Ok(new { success = true, message = $"Az adatbázis '{databaseName}' sikeresen létrehozva" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hiba az adatbázis létrehozásakor");
            return StatusCode(500, new { success = false, message = "Hiba történt az adatbázis létrehozásakor", error = ex.Message });
        }
    }

    [HttpGet("info")]
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

            return Ok(new
            {
                success = true,
                database = builder.Database,
                host = builder.Host,
                port = builder.Port,
                version = version?.ToString(),
                size = dbSize?.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hiba az adatbázis információk lekérésekor");
            return StatusCode(500, new { success = false, message = "Hiba történt", error = ex.Message });
        }
    }
}