using Kitcheer.API.Entities;
using Kitcheer.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Kitcheer.API.Functions;

public class StoredProductFunctions
{
    private readonly ApplicationDbContext _context;

    public StoredProductFunctions(ApplicationDbContext context)
 {
  _context = context;
    }

    public async Task<object> GetAllAsync()
    {
     try
     {
     var products = await _context.StoredProducts
  .Include(sp => sp.ProductTemplate)
       .Include(sp => sp.StorageLocation)
      .ToListAsync();

     return new { success = true, data = products };
  }
   catch (Exception ex)
   {
      return new { success = false, error = ex.Message };
        }
    }

  public async Task<object> GetByIdAsync(int id)
 {
   try
      {
      var product = await _context.StoredProducts
    .Include(sp => sp.ProductTemplate)
 .Include(sp => sp.StorageLocation)
      .FirstOrDefaultAsync(sp => sp.Id == id);

         if (product == null)
 return new { success = false, error = "Tárolt termék nem található" };

     return new { success = true, data = product };
        }
        catch (Exception ex)
     {
     return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> GetByStorageLocationAsync(int storageLocationId)
    {
        try
    {
     var products = await _context.StoredProducts
  .Include(sp => sp.ProductTemplate)
  .Include(sp => sp.StorageLocation)
   .Where(sp => sp.StorageLocationId == storageLocationId)
       .ToListAsync();

    return new { success = true, data = products };
        }
      catch (Exception ex)
     {
     return new { success = false, error = ex.Message };
    }
    }

    public async Task<object> GetExpiringAsync(int days = 7)
    {
        try
   {
 var expiryDate = DateTime.Now.AddDays(days);
        var products = await _context.StoredProducts
         .Include(sp => sp.ProductTemplate)
       .Include(sp => sp.StorageLocation)
       .Where(sp => sp.ExpiryDate.HasValue && sp.ExpiryDate <= expiryDate)
   .OrderBy(sp => sp.ExpiryDate)
      .ToListAsync();

      return new { success = true, data = products };
        }
      catch (Exception ex)
  {
          return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> CreateAsync(StoredProduct product)
    {
     try
        {
      // Ellenőrizzük, hogy létezik-e a ProductTemplate és StorageLocation
  var templateExists = await _context.ProductTemplates.AnyAsync(pt => pt.Id == product.ProductTemplateId);
            var locationExists = await _context.StorageLocations.AnyAsync(sl => sl.Id == product.StorageLocationId);

   if (!templateExists)
     return new { success = false, error = "Termék sablon nem található" };
     if (!locationExists)
   return new { success = false, error = "Tárolóhely nem található" };

      _context.StoredProducts.Add(product);
         await _context.SaveChangesAsync();

        // Reload with includes
       var created = await _context.StoredProducts
      .Include(sp => sp.ProductTemplate)
          .Include(sp => sp.StorageLocation)
            .FirstAsync(sp => sp.Id == product.Id);

       return new { success = true, data = created };
        }
    catch (Exception ex)
        {
     return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> UpdateAsync(int id, StoredProduct product)
    {
        try
        {
            var existing = await _context.StoredProducts.FindAsync(id);
            if (existing == null)
       return new { success = false, error = "Tárolt termék nem található" };

      // Ellenőrizzük, hogy létezik-e a ProductTemplate és StorageLocation
        var templateExists = await _context.ProductTemplates.AnyAsync(pt => pt.Id == product.ProductTemplateId);
    var locationExists = await _context.StorageLocations.AnyAsync(sl => sl.Id == product.StorageLocationId);

       if (!templateExists)
     return new { success = false, error = "Termék sablon nem található" };
      if (!locationExists)
      return new { success = false, error = "Tárolóhely nem található" };

          existing.ProductTemplateId = product.ProductTemplateId;
       existing.StorageLocationId = product.StorageLocationId;
       existing.ExpiryDate = product.ExpiryDate;
     existing.Quantity = product.Quantity;
      existing.Unit = product.Unit;
    existing.PurchaseDate = product.PurchaseDate;
     existing.ProductDetails = product.ProductDetails;

         await _context.SaveChangesAsync();

         // Reload with includes
       var updated = await _context.StoredProducts
     .Include(sp => sp.ProductTemplate)
      .Include(sp => sp.StorageLocation)
         .FirstAsync(sp => sp.Id == id);

        return new { success = true, data = updated };
   }
    catch (Exception ex)
        {
       return new { success = false, error = ex.Message };
        }
    }

  public async Task<object> DeleteAsync(int id)
    {
        try
        {
      var product = await _context.StoredProducts.FindAsync(id);
 if (product == null)
     return new { success = false, error = "Tárolt termék nem található" };

      product.DeleteFl = true;
     await _context.SaveChangesAsync();
     return new { success = true, message = "Tárolt termék sikeresen törölve" };
      }
        catch (Exception ex)
 {
      return new { success = false, error = ex.Message };
     }
    }

    public async Task<object> MoveProductAsync(int id, int newStorageLocationId, decimal? newQuantity = null)
    {
        try
        {
         var product = await _context.StoredProducts.FindAsync(id);
      if (product == null)
        return new { success = false, error = "Tárolt termék nem található" };

       var newLocation = await _context.StorageLocations.FindAsync(newStorageLocationId);
       if (newLocation == null)
return new { success = false, error = "Új tárolóhely nem található" };

   var oldLocationId = product.StorageLocationId;
            var oldQuantity = product.Quantity;

     product.StorageLocationId = newStorageLocationId;
         if (newQuantity.HasValue)
       product.Quantity = newQuantity.Value;

        await _context.SaveChangesAsync();

    // Create movement record
     var movement = new ProductMovement
       {
 StoredProductId = id,
         MovementType = Enums.MovementType.Moved,
         FromStorageLocationId = oldLocationId,
         ToStorageLocationId = newStorageLocationId,
  Quantity = newQuantity ?? oldQuantity,
         Unit = product.Unit,
         MovementData = System.Text.Json.JsonSerializer.Serialize(new 
            {
       reason = "Manual move",
      oldQuantity = oldQuantity,
     newQuantity = newQuantity ?? oldQuantity
            })
      };

 _context.ProductMovements.Add(movement);
       await _context.SaveChangesAsync();

    return new { success = true, message = "Termék sikeresen áthelyezve" };
        }
      catch (Exception ex)
  {
 return new { success = false, error = ex.Message };
  }
    }
}