using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kitcheer.API.Enums;

namespace Kitcheer.API.Entities;

public class ProductTemplate : BaseEntity
{
    [MaxLength(100)]
    public string? Brand { get; set; }
 
  [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Barcode { get; set; }
    
    public ProductType ProductType { get; set; }
    
    [Column(TypeName = "decimal(10,3)")]
    public decimal MinimumQuantity { get; set; } = 0;
    
    [MaxLength(20)]
    public string? DefaultUnit { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string? ProductData { get; set; }
    
    public virtual ICollection<StoredProduct> StoredProducts { get; set; } = new List<StoredProduct>();
    public virtual ICollection<ShoppingListItem> ShoppingListItems { get; set; } = new List<ShoppingListItem>();
}