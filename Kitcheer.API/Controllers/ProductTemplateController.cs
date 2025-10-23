using Microsoft.AspNetCore.Mvc;
using Kitcheer.API.Functions;
using Kitcheer.API.Entities;

namespace Kitcheer.API.Controllers;

/// <summary>
/// Termék sablonok kezelését biztosító API végpontok
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductTemplateController : ControllerBase
{
    private readonly ProductTemplateFunctions _functions;

 public ProductTemplateController(ProductTemplateFunctions functions)
    {
        _functions = functions;
 }

    /// <summary>
    /// Összes termék sablon lekérése
    /// </summary>
    /// <returns>Termék sablonok listája kapcsolódó tárolt termékekkel</returns>
    /// <response code="200">Sikeres lekérés</response>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _functions.GetAllAsync();
 return Ok(result);
    }

    /// <summary>
    /// Termék sablon lekérése ID alapján
    /// </summary>
    /// <param name="id">Termék sablon azonosító</param>
/// <returns>Termék sablon adatok kapcsolódó tárolt termékekkel</returns>
    /// <response code="200">Sikeres lekérés</response>
  /// <response code="404">Termék sablon nem található</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
    var result = await _functions.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
  /// Alacsony készletű termékek lekérése
    /// </summary>
    /// <returns>Termékek ahol a teljes készlet a minimum mennyiség alatt van</returns>
    /// <response code="200">Sikeres lekérés</response>
    [HttpGet("low-stock")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetLowStock()
    {
 var result = await _functions.GetLowStockAsync();
      return Ok(result);
    }

    /// <summary>
    /// Új termék sablon létrehozása
    /// </summary>
    /// <param name="template">Termék sablon adatok</param>
    /// <returns>Létrehozott termék sablon</returns>
 /// <response code="200">Sikeres létrehozás</response>
    /// <response code="400">Hibás adatok vagy már létezik ilyen márka+név kombináció</response>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] ProductTemplate template)
    {
        var result = await _functions.CreateAsync(template);
      return Ok(result);
    }

    /// <summary>
    /// Termék sablon frissítése
    /// </summary>
    /// <param name="id">Termék sablon azonosító</param>
    /// <param name="template">Frissített termék sablon adatok</param>
    /// <returns>Frissített termék sablon</returns>
    /// <response code="200">Sikeres frissítés</response>
    /// <response code="404">Termék sablon nem található</response>
    /// <response code="400">Már létezik ilyen márka+név kombináció</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] ProductTemplate template)
    {
 var result = await _functions.UpdateAsync(id, template);
     return Ok(result);
    }

    /// <summary>
    /// Termék sablon törlése (soft delete)
    /// </summary>
    /// <param name="id">Termék sablon azonosító</param>
    /// <returns>Törlés eredménye</returns>
    /// <response code="200">Sikeres törlés</response>
    /// <response code="404">Termék sablon nem található</response>
    /// <response code="400">Nem törölhető, mert használják aktív termékek</response>
    [HttpDelete("{id}")]
  [ProducesResponseType(200)]
    [ProducesResponseType(404)]
 [ProducesResponseType(400)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _functions.DeleteAsync(id);
        return Ok(result);
    }
}