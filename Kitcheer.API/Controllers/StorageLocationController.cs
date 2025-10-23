using Microsoft.AspNetCore.Mvc;
using Kitcheer.API.Functions;
using Kitcheer.API.Entities;

namespace Kitcheer.API.Controllers;

/// <summary>
/// Tárolóhelyek (hűtő, kamra, fagyasztó) kezelését biztosító API végpontok
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StorageLocationController : ControllerBase
{
    private readonly StorageLocationFunctions _functions;

    public StorageLocationController(StorageLocationFunctions functions)
    {
        _functions = functions;
    }

    /// <summary>
    /// Összes tárolóhely lekérése
 /// </summary>
    /// <returns>Tárolóhelyek listája</returns>
    /// <response code="200">Sikeres lekérés</response>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _functions.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Tárolóhely lekérése ID alapján
 /// </summary>
    /// <param name="id">Tárolóhely azonosító</param>
 /// <returns>Tárolóhely adatok</returns>
    /// <response code="200">Sikeres lekérés</response>
  /// <response code="404">Tárolóhely nem található</response>
    [HttpGet("{id}")]
 [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
      var result = await _functions.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Új tárolóhely létrehozása
    /// </summary>
    /// <param name="location">Tárolóhely adatok</param>
    /// <returns>Létrehozott tárolóhely</returns>
    /// <response code="200">Sikeres létrehozás</response>
 /// <response code="400">Hibás adatok</response>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] StorageLocation location)
    {
        var result = await _functions.CreateAsync(location);
        return Ok(result);
    }

    /// <summary>
    /// Tárolóhely frissítése
    /// </summary>
    /// <param name="id">Tárolóhely azonosító</param>
    /// <param name="location">Frissített tárolóhely adatok</param>
    /// <returns>Frissített tárolóhely</returns>
 /// <response code="200">Sikeres frissítés</response>
    /// <response code="404">Tárolóhely nem található</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] StorageLocation location)
    {
        var result = await _functions.UpdateAsync(id, location);
        return Ok(result);
  }

    /// <summary>
    /// Tárolóhely törlése (soft delete)
    /// </summary>
    /// <param name="id">Tárolóhely azonosító</param>
 /// <returns>Törlés eredménye</returns>
 /// <response code="200">Sikeres törlés</response>
    /// <response code="404">Tárolóhely nem található</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
  [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _functions.DeleteAsync(id);
        return Ok(result);
    }
}