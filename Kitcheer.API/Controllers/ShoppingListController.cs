using Microsoft.AspNetCore.Mvc;
using Kitcheer.API.Functions;
using Kitcheer.API.Entities;

namespace Kitcheer.API.Controllers;

/// <summary>
/// Bevásárlási listák kezelését biztosító API végpontok
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ShoppingListController : ControllerBase
{
    private readonly ShoppingListFunctions _functions;

 public ShoppingListController(ShoppingListFunctions functions)
 {
        _functions = functions;
    }

 /// <summary>
    /// Összes bevásárlási lista lekérése
    /// </summary>
    /// <returns>Bevásárlási listák tételekkel együtt</returns>
    /// <response code="200">Sikeres lekérés</response>
    [HttpGet]
    [ProducesResponseType(200)]
  public async Task<IActionResult> GetAll()
 {
        var result = await _functions.GetAllAsync();
  return Ok(result);
    }

    /// <summary>
 /// Aktív bevásárlási listák lekérése
  /// </summary>
    /// <returns>Csak az aktív bevásárlási listák</returns>
    /// <response code="200">Sikeres lekérés</response>
    [HttpGet("active")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetActive()
    {
        var result = await _functions.GetActiveAsync();
        return Ok(result);
}

    /// <summary>
  /// Bevásárlási lista lekérése ID alapján
    /// </summary>
    /// <param name="id">Bevásárlási lista azonosító</param>
    /// <returns>Bevásárlási lista tételekkel együtt</returns>
    /// <response code="200">Sikeres lekérés</response>
    /// <response code="404">Bevásárlási lista nem található</response>
    [HttpGet("{id}")]
  [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
 var result = await _functions.GetByIdAsync(id);
   return Ok(result);
  }

    /// <summary>
    /// Új bevásárlási lista létrehozása
    /// </summary>
    /// <param name="list">Bevásárlási lista adatok</param>
    /// <returns>Létrehozott bevásárlási lista</returns>
    /// <response code="200">Sikeres létrehozás</response>
    /// <response code="400">Hibás adatok</response>
    [HttpPost]
    [ProducesResponseType(200)]
  [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] ShoppingList list)
    {
   var result = await _functions.CreateAsync(list);
        return Ok(result);
    }

    /// <summary>
    /// Bevásárlási lista frissítése
    /// </summary>
    /// <param name="id">Bevásárlási lista azonosító</param>
    /// <param name="list">Frissített lista adatok</param>
    /// <returns>Frissített bevásárlási lista</returns>
    /// <response code="200">Sikeres frissítés</response>
    /// <response code="404">Bevásárlási lista nem található</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] ShoppingList list)
    {
        var result = await _functions.UpdateAsync(id, list);
        return Ok(result);
  }

    /// <summary>
    /// Bevásárlási lista törlése (soft delete)
    /// </summary>
    /// <param name="id">Bevásárlási lista azonosító</param>
    /// <returns>Törlés eredménye</returns>
    /// <response code="200">Sikeres törlés</response>
    /// <response code="404">Bevásárlási lista nem található</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
  public async Task<IActionResult> Delete(int id)
  {
var result = await _functions.DeleteAsync(id);
  return Ok(result);
 }

    /// <summary>
    /// Tétel hozzáadása bevásárlási listához
    /// </summary>
    /// <param name="listId">Bevásárlási lista azonosító</param>
    /// <param name="item">Hozzáadandó tétel adatok</param>
    /// <returns>Hozzáadott tétel</returns>
    /// <response code="200">Sikeres hozzáadás</response>
    /// <response code="404">Lista vagy termék sablon nem található</response>
    /// <response code="400">Hibás adatok</response>
  [HttpPost("{listId}/items")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
 public async Task<IActionResult> AddItem(int listId, [FromBody] ShoppingListItem item)
    {
    var result = await _functions.AddItemAsync(listId, item);
      return Ok(result);
  }

    /// <summary>
    /// Bevásárlási lista tétel frissítése
    /// </summary>
    /// <param name="itemId">Tétel azonosító</param>
    /// <param name="item">Frissített tétel adatok</param>
    /// <returns>Frissített tétel</returns>
    /// <response code="200">Sikeres frissítés</response>
    /// <response code="404">Tétel vagy termék sablon nem található</response>
    /// <response code="400">Hibás adatok</response>
    [HttpPut("items/{itemId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateItem(int itemId, [FromBody] ShoppingListItem item)
    {
  var result = await _functions.UpdateItemAsync(itemId, item);
        return Ok(result);
    }

    /// <summary>
    /// Tétel eltávolítása bevásárlási listából (soft delete)
    /// </summary>
    /// <param name="itemId">Tétel azonosító</param>
    /// <returns>Eltávolítás eredménye</returns>
    /// <response code="200">Sikeres eltávolítás</response>
    /// <response code="404">Tétel nem található</response>
    [HttpDelete("items/{itemId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        var result = await _functions.RemoveItemAsync(itemId);
 return Ok(result);
    }

    /// <summary>
    /// Tétel megvásárolt állapotának módosítása
    /// </summary>
    /// <param name="itemId">Tétel azonosító</param>
    /// <param name="request">Vásárlási állapot kérés</param>
    /// <returns>Módosítás eredménye</returns>
    /// <response code="200">Sikeres módosítás</response>
    /// <response code="404">Tétel nem található</response>
    [HttpPost("items/{itemId}/purchase")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> MarkItemPurchased(int itemId, [FromBody] MarkPurchasedRequest request)
    {
        var result = await _functions.MarkItemPurchasedAsync(itemId, request.IsPurchased);
   return Ok(result);
    }

    /// <summary>
    /// Alacsony készletű termékek automatikus hozzáadása a listához
    /// </summary>
    /// <param name="listId">Bevásárlási lista azonosító</param>
    /// <returns>Hozzáadott tételek listája</returns>
    /// <response code="200">Sikeres hozzáadás</response>
    /// <response code="404">Bevásárlási lista nem található</response>
    [HttpPost("{listId}/auto-add-low-stock")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
public async Task<IActionResult> AutoAddLowStockItems(int listId)
    {
      var result = await _functions.AutoAddLowStockItemsAsync(listId);
        return Ok(result);
    }
}

/// <summary>
/// Tétel vásárlási állapot kérés
/// </summary>
public class MarkPurchasedRequest
{
    /// <summary>
    /// Megvásárolva-e (alapértelmezett: true)
    /// </summary>
 public bool IsPurchased { get; set; } = true;
}