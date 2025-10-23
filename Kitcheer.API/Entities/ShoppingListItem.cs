using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kitcheer.API.Entities;

public class ShoppingListItem : BaseEntity
{
    [Required]
    public int ShoppingListId { get; set; }
    
    public int? ProductTemplateId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Brand { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,3)")]
    public decimal Quantity { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;
 
    [Required]
public bool IsPurchased { get; set; } = false;
    
    [Column(TypeName = "jsonb")]
    public string? ItemData { get; set; }
    
    [ForeignKey("ShoppingListId")]
    public virtual ShoppingList ShoppingList { get; set; } = null!;
    
    [ForeignKey("ProductTemplateId")]
    public virtual ProductTemplate? ProductTemplate { get; set; }
}