using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kitcheer.API.Enums;

namespace Kitcheer.API.Entities;

public class ProductMovement : BaseEntity
{
    [Required]
    public int StoredProductId { get; set; }
    
    public MovementType MovementType { get; set; }
    
    public int? FromStorageLocationId { get; set; }
    
    public int? ToStorageLocationId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,3)")]
    public decimal Quantity { get; set; }
    
    [Required]
[MaxLength(20)]
    public string Unit { get; set; } = string.Empty;
    
    [Column(TypeName = "jsonb")]
    public string? MovementData { get; set; }
    
    [ForeignKey("StoredProductId")]
    public virtual StoredProduct StoredProduct { get; set; } = null!;
  
    [ForeignKey("FromStorageLocationId")]
    public virtual StorageLocation? FromStorageLocation { get; set; }
    
    [ForeignKey("ToStorageLocationId")]
    public virtual StorageLocation? ToStorageLocation { get; set; }
}