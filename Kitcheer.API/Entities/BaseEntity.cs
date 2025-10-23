using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Kitcheer.API.Entities;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public bool DeleteFl { get; set; } = false;
    
    [Column(TypeName = "jsonb")]
    public string RekordChange { get; set; } = JsonSerializer.Serialize(new { LastModifiedDate = DateTime.Now });
    
    public void UpdateRekordChange()
    {
 RekordChange = JsonSerializer.Serialize(new { LastModifiedDate = DateTime.Now });
    }
}