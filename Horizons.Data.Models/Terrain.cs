using System.ComponentModel.DataAnnotations;
using static Horizons.GCommon.EntityConstrains.Terrain;

namespace Horizons.Data.Models;

public class Terrain
{
    [Required]
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = null!;
    
    public virtual ICollection<Destination> Destinations { get; set; } = new HashSet<Destination>();
}