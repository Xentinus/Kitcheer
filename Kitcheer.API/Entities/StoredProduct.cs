using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kitcheer.API.Entities;

public class StoredProduct : BaseEntity
{
    [Required]
    public int ProductTemplateId { get; set; }
    
    [Required]
    public int StorageLocationId { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,3)")]
    public decimal Quantity { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;
    
  public DateTime? PurchaseDate { get; set; }
    
  [Column(TypeName = "jsonb")]
    public string? ProductDetails { get; set; }
    
    [ForeignKey("ProductTemplateId")]
    public virtual ProductTemplate ProductTemplate { get; set; } = null!;
  
    [ForeignKey("StorageLocationId")]
    public virtual StorageLocation StorageLocation { get; set; } = null!;
    
  public virtual ICollection<ProductMovement> ProductMovements { get; set; } = new List<ProductMovement>();
}