using Microsoft.AspNetCore.Mvc;
using Kitcheer.API.Functions;
using Kitcheer.API.Entities;

namespace Kitcheer.API.Controllers;

/// <summary>
/// Tárolt termékek kezelését biztosító API végpontok
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StoredProductController : ControllerBase
{
  private readonly StoredProductFunctions _functions;

  public StoredProductController(StoredProductFunctions functions)
  {
      _functions = functions;
 }

    /// <summary>
    /// Összes tárolt termék lekérése
    /// </summary>
    /// <returns>Tárolt termékek listája kapcsolódó sablonokkal és tárolóhelyekkel</returns>
    /// <response code="200">Sikeres lekérés</response>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAll()
    {
 var result = await _functions.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Tárolt termék lekérése ID alapján
    /// </summary>
    /// <param name="id">Tárolt termék azonosító</param>
 /// <returns>Tárolt termék adatok</returns>
    /// <response code="200">Sikeres lekérés</response>
    /// <response code="404">Tárolt termék nem található</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
  {
   var result = await _functions.GetByIdAsync(id);
  return Ok(result);
    }

    /// <summary>
    /// Adott tárolóhelyen lévő termékek lekérése
    /// </summary>
    /// <param name="storageLocationId">Tárolóhely azonosító</param>
    /// <returns>A tárolóhelyen lévő termékek listája</returns>
    /// <response code="200">Sikeres lekérés</response>
    /// <response code="404">Tárolóhely nem található</response>
    [HttpGet("storage/{storageLocationId}")]
    [ProducesResponseType(200)]
  [ProducesResponseType(404)]
    public async Task<IActionResult> GetByStorageLocation(int storageLocationId)
    {
   var result = await _functions.GetByStorageLocationAsync(storageLocationId);
     return Ok(result);
    }

    /// <summary>
    /// Hamarosan lejáró termékek lekérése
    /// </summary>
 /// <param name="days">Hány napon belül járnak le (alapértelmezett: 7 nap)</param>
    /// <returns>Lejáró termékek listája</returns>
    /// <response code="200">Sikeres lekérés</response>
    [HttpGet("expiring")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetExpiring([FromQuery] int days = 7)
  {
   var result = await _functions.GetExpiringAsync(days);
return Ok(result);
    }

    /// <summary>
    /// Új tárolt termék létrehozása
    /// </summary>
    /// <param name="product">Tárolt termék adatok</param>
    /// <returns>Létrehozott tárolt termék</returns>
    /// <response code="200">Sikeres létrehozás</response>
    /// <response code="400">Hibás adatok vagy nem létező sablon/tárolóhely</response>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
 public async Task<IActionResult> Create([FromBody] StoredProduct product)
    {
        var result = await _functions.CreateAsync(product);
 return Ok(result);
    }

    /// <summary>
    /// Tárolt termék frissítése
    /// </summary>
    /// <param name="id">Tárolt termék azonosító</param>
    /// <param name="product">Frissített termék adatok</param>
    /// <returns>Frissített tárolt termék</returns>
    /// <response code="200">Sikeres frissítés</response>
    /// <response code="404">Tárolt termék nem található</response>
    /// <response code="400">Hibás adatok vagy nem létező sablon/tárolóhely</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
  public async Task<IActionResult> Update(int id, [FromBody] StoredProduct product)
    {
      var result = await _functions.UpdateAsync(id, product);
        return Ok(result);
    }

    /// <summary>
    /// Tárolt termék törlése (soft delete)
 /// </summary>
    /// <param name="id">Tárolt termék azonosító</param>
    /// <returns>Törlés eredménye</returns>
    /// <response code="200">Sikeres törlés</response>
 /// <response code="404">Tárolt termék nem található</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
  {
   var result = await _functions.DeleteAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Termék áthelyezése másik tárolóhelyre
    /// </summary>
    /// <param name="id">Tárolt termék azonosító</param>
    /// <param name="request">Áthelyezési kérés adatok</param>
    /// <returns>Áthelyezés eredménye</returns>
    /// <response code="200">Sikeres áthelyezés</response>
    /// <response code="404">Termék vagy tárolóhely nem található</response>
    [HttpPost("{id}/move")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> MoveProduct(int id, [FromBody] MoveProductRequest request)
  {
   var result = await _functions.MoveProductAsync(id, request.NewStorageLocationId, request.NewQuantity);
        return Ok(result);
    }
}

/// <summary>
/// Termék áthelyezési kérés
/// </summary>
public class MoveProductRequest
{
    /// <summary>
    /// Új tárolóhely azonosító
    /// </summary>
    public int NewStorageLocationId { get; set; }
    
    /// <summary>
    /// Új mennyiség (opcionális)
    /// </summary>
    public decimal? NewQuantity { get; set; }
}