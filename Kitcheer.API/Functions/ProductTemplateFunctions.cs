using Kitcheer.API.Entities;
using Kitcheer.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Kitcheer.API.Functions;

public class ProductTemplateFunctions
{
    private readonly ApplicationDbContext _context;

  public ProductTemplateFunctions(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetAllAsync()
    {
        try
  {
          var templates = await _context.ProductTemplates
  .Include(p => p.StoredProducts)
      .ThenInclude(sp => sp.StorageLocation)
             .ToListAsync();

            return new { success = true, data = templates };
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
        var template = await _context.ProductTemplates
          .Include(p => p.StoredProducts)
.ThenInclude(sp => sp.StorageLocation)
   .FirstOrDefaultAsync(p => p.Id == id);

        if (template == null)
     return new { success = false, error = "Termék sablon nem található" };

            return new { success = true, data = template };
      }
   catch (Exception ex)
        {
       return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> CreateAsync(ProductTemplate template)
    {
        try
        {
  // Ellenőrizzük, hogy létezik-e már ilyen Brand + Name kombináció
      var existing = await _context.ProductTemplates
        .FirstOrDefaultAsync(p => p.Brand == template.Brand && p.Name == template.Name);

       if (existing != null)
  return new { success = false, error = "Ilyen márka és név kombinációjú termék már létezik" };

      _context.ProductTemplates.Add(template);
        await _context.SaveChangesAsync();
       return new { success = true, data = template };
        }
        catch (Exception ex)
      {
      return new { success = false, error = ex.Message };
  }
    }

    public async Task<object> UpdateAsync(int id, ProductTemplate template)
  {
        try
    {
   var existing = await _context.ProductTemplates.FindAsync(id);
    if (existing == null)
   return new { success = false, error = "Termék sablon nem található" };

  // Ellenőrizzük az egyediséget (kivéve a jelenlegi rekordot)
    var duplicate = await _context.ProductTemplates
         .FirstOrDefaultAsync(p => p.Brand == template.Brand && p.Name == template.Name && p.Id != id);

            if (duplicate != null)
    return new { success = false, error = "Ilyen márka és név kombinációjú termék már létezik" };

         existing.Brand = template.Brand;
       existing.Name = template.Name;
       existing.Barcode = template.Barcode;
       existing.ProductType = template.ProductType;
         existing.MinimumQuantity = template.MinimumQuantity;
    existing.DefaultUnit = template.DefaultUnit;
         existing.ProductData = template.ProductData;

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
   var template = await _context.ProductTemplates.FindAsync(id);
        if (template == null)
                return new { success = false, error = "Termék sablon nem található" };

            template.DeleteFl = true;
      await _context.SaveChangesAsync();
   return new { success = true, message = "Termék sablon sikeresen törölve" };
        }
   catch (Exception ex)
        {
 return new { success = false, error = ex.Message };
  }
    }

    public async Task<object> GetLowStockAsync()
    {
        try
 {
            var lowStockTemplates = await _context.ProductTemplates
     .Where(pt => _context.StoredProducts
     .Where(sp => sp.ProductTemplateId == pt.Id)
       .Sum(sp => sp.Quantity) < pt.MinimumQuantity)
      .Include(pt => pt.StoredProducts)
     .ThenInclude(sp => sp.StorageLocation)
  .ToListAsync();

            return new { success = true, data = lowStockTemplates };
  }
        catch (Exception ex)
     {
     return new { success = false, error = ex.Message };
        }
  }
}