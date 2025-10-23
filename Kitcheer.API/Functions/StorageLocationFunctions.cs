using Kitcheer.API.Entities;
using Kitcheer.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Kitcheer.API.Functions;

public class StorageLocationFunctions
{
    private readonly ApplicationDbContext _context;

    public StorageLocationFunctions(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetAllAsync()
    {
        try
      {
         var locations = await _context.StorageLocations.ToListAsync();
       return new { success = true, data = locations };
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
          var location = await _context.StorageLocations.FindAsync(id);
  if (location == null)
                return new { success = false, error = "Tárolóhely nem található" };

      return new { success = true, data = location };
        }
    catch (Exception ex)
        {
            return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> CreateAsync(StorageLocation location)
    {
        try
        {
  _context.StorageLocations.Add(location);
            await _context.SaveChangesAsync();
    return new { success = true, data = location };
        }
        catch (Exception ex)
    {
         return new { success = false, error = ex.Message };
        }
    }

    public async Task<object> UpdateAsync(int id, StorageLocation location)
  {
        try
        {
      var existing = await _context.StorageLocations.FindAsync(id);
        if (existing == null)
          return new { success = false, error = "Tárolóhely nem található" };

            existing.Name = location.Name;
        existing.Type = location.Type;
            existing.AdditionalData = location.AdditionalData;

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
    var location = await _context.StorageLocations.FindAsync(id);
            if (location == null)
  return new { success = false, error = "Tárolóhely nem található" };

       location.DeleteFl = true;
        await _context.SaveChangesAsync();
            return new { success = true, message = "Tárolóhely sikeresen törölve" };
        }
      catch (Exception ex)
        {
       return new { success = false, error = ex.Message };
}
    }
}