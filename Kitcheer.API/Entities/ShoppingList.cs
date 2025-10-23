using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kitcheer.API.Entities;

public class ShoppingList : BaseEntity
{
[Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public bool IsActive { get; set; } = true;
  
    [Column(TypeName = "jsonb")]
  public string? ListData { get; set; }
    
    public virtual ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
}