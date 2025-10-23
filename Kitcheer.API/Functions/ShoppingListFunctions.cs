using Kitcheer.API.Entities;
using Kitcheer.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Kitcheer.API.Functions;

public class ShoppingListFunctions
{
    private readonly ApplicationDbContext _context;

public ShoppingListFunctions(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetAllAsync()
    {
      try
        {
            var lists = await _context.ShoppingLists
      .Include(sl => sl.Items)
          .ThenInclude(sli => sli.ProductTemplate)
                .ToListAsync();

          return new { success = true, data = lists };
        }
     catch (Exception ex)
   {
            return new { success = false, error = ex.Message };
      }
    }

public async Task<object> GetActiveAsync()
    {
      try
        {
            var activeLists = await _context.ShoppingLists
   .Include(sl => sl.Items)
                .ThenInclude(sli => sli.ProductTemplate)
     .Where(sl => sl.IsActive)
    .ToListAsync();

  return new { success = true, data = activeLists };
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
       var list = await _context.ShoppingLists
  .Include(sl => sl.Items)
       .ThenInclude(sli => sli.ProductTemplate)
   .FirstOrDefaultAsync(sl => sl.Id == id);

            if (list == null)
      return new { success = false, error = "Bevásárlási lista nem található" };

            return new { success = true, data = list };
        }
    catch (Exception ex)
        {
        return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> CreateAsync(ShoppingList list)
    {
  try
        {
            _context.ShoppingLists.Add(list);
  await _context.SaveChangesAsync();
     return new { success = true, data = list };
        }
        catch (Exception ex)
{
         return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> UpdateAsync(int id, ShoppingList list)
    {
   try
 {
     var existing = await _context.ShoppingLists.FindAsync(id);
    if (existing == null)
     return new { success = false, error = "Bevásárlási lista nem található" };

            existing.Name = list.Name;
         existing.IsActive = list.IsActive;
            existing.ListData = list.ListData;

       await _context.SaveChangesAsync();
            return new { success = true, data = existing };
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
var list = await _context.ShoppingLists.FindAsync(id);
     if (list == null)
     return new { success = false, error = "Bevásárlási lista nem található" };

            list.DeleteFl = true;
  await _context.SaveChangesAsync();
        return new { success = true, message = "Bevásárlási lista sikeresen törölve" };
        }
   catch (Exception ex)
        {
   return new { success = false, error = ex.Message };
 }
    }

    public async Task<object> AddItemAsync(int listId, ShoppingListItem item)
    {
        try
        {
            var list = await _context.ShoppingLists.FindAsync(listId);
 if (list == null)
     return new { success = false, error = "Bevásárlási lista nem található" };

         if (item.ProductTemplateId.HasValue)
         {
      var templateExists = await _context.ProductTemplates.AnyAsync(pt => pt.Id == item.ProductTemplateId);
     if (!templateExists)
      return new { success = false, error = "Termék sablon nem található" };
        }

            item.ShoppingListId = listId;
            _context.ShoppingListItems.Add(item);
         await _context.SaveChangesAsync();

            // Reload with includes
            var created = await _context.ShoppingListItems
        .Include(sli => sli.ProductTemplate)
                .FirstAsync(sli => sli.Id == item.Id);

          return new { success = true, data = created };
        }
        catch (Exception ex)
   {
         return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> UpdateItemAsync(int itemId, ShoppingListItem item)
    {
    try
        {
            var existing = await _context.ShoppingListItems.FindAsync(itemId);
     if (existing == null)
    return new { success = false, error = "Bevásárlási lista tétel nem található" };

      if (item.ProductTemplateId.HasValue)
         {
 var templateExists = await _context.ProductTemplates.AnyAsync(pt => pt.Id == item.ProductTemplateId);
     if (!templateExists)
     return new { success = false, error = "Termék sablon nem található" };
            }

  existing.ProductTemplateId = item.ProductTemplateId;
            existing.Name = item.Name;
            existing.Brand = item.Brand;
  existing.Quantity = item.Quantity;
existing.Unit = item.Unit;
   existing.IsPurchased = item.IsPurchased;
        existing.ItemData = item.ItemData;

 await _context.SaveChangesAsync();

      // Reload with includes
       var updated = await _context.ShoppingListItems
     .Include(sli => sli.ProductTemplate)
                .FirstAsync(sli => sli.Id == itemId);

            return new { success = true, data = updated };
        }
        catch (Exception ex)
        {
 return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> RemoveItemAsync(int itemId)
    {
        try
    {
   var item = await _context.ShoppingListItems.FindAsync(itemId);
            if (item == null)
                return new { success = false, error = "Bevásárlási lista tétel nem található" };

       item.DeleteFl = true;
       await _context.SaveChangesAsync();
        return new { success = true, message = "Tétel sikeresen törölve" };
        }
        catch (Exception ex)
        {
  return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> MarkItemPurchasedAsync(int itemId, bool isPurchased = true)
    {
        try
        {
            var item = await _context.ShoppingListItems.FindAsync(itemId);
            if (item == null)
      return new { success = false, error = "Bevásárlási lista tétel nem található" };

    item.IsPurchased = isPurchased;
      await _context.SaveChangesAsync();
      return new { success = true, message = $"Tétel {(isPurchased ? "megvásároltnak" : "nem vásároltnak")} jelölve" };
        }
        catch (Exception ex)
      {
        return new { success = false, error = ex.Message };
}
    }

    public async Task<object> AutoAddLowStockItemsAsync(int listId)
 {
        try
        {
 var list = await _context.ShoppingLists.FindAsync(listId);
     if (list == null)
           return new { success = false, error = "Bevásárlási lista nem található" };

    // Find templates where total stored quantity is below minimum
       var lowStockTemplates = await _context.ProductTemplates
    .Where(pt => _context.StoredProducts
           .Where(sp => sp.ProductTemplateId == pt.Id)
       .Sum(sp => sp.Quantity) < pt.MinimumQuantity)
       .Where(pt => !_context.ShoppingListItems
 .Any(sli => sli.ShoppingListId == listId && sli.ProductTemplateId == pt.Id && !sli.IsPurchased))
      .ToListAsync();

         var addedItems = new List<ShoppingListItem>();
            foreach (var template in lowStockTemplates)
            {
      var currentStock = await _context.StoredProducts
              .Where(sp => sp.ProductTemplateId == template.Id)
 .SumAsync(sp => sp.Quantity);

                var neededQuantity = template.MinimumQuantity - currentStock;

      var item = new ShoppingListItem
  {
          ShoppingListId = listId,
       ProductTemplateId = template.Id,
        Name = template.Name,
    Brand = template.Brand,
                Quantity = neededQuantity,
           Unit = template.DefaultUnit ?? "db",
      IsPurchased = false,
 ItemData = JsonSerializer.Serialize(new
            {
         autoAddedReason = "minimum_quantity",
         autoAddedDate = DateTime.UtcNow,
            currentStock = currentStock,
   minimumQuantity = template.MinimumQuantity
        })
                };

       _context.ShoppingListItems.Add(item);
    addedItems.Add(item);
     }

      await _context.SaveChangesAsync();
 return new { success = true, message = $"{addedItems.Count} tétel automatikusan hozzáadva", data = addedItems };
 }
  catch (Exception ex)
   {
return new { success = false, error = ex.Message };
        }
    }
}