using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kitcheer.API.Enums;

namespace Kitcheer.API.Entities;

public class StorageLocation : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public StorageType Type { get; set; }
    
 [Column(TypeName = "jsonb")]
    public string? AdditionalData { get; set; }
    
    public virtual ICollection<StoredProduct> StoredProducts { get; set; } = new List<StoredProduct>();
}