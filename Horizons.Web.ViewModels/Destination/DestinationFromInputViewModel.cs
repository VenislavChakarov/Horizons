using System.ComponentModel.DataAnnotations;
using static Horizons.GCommon.EntityConstrains.Destination;

namespace Horizons.Web.ViewModels.Destination;

public class DestinationFromInputViewModel
{
    [Required]
    [MinLength(NameMinLength)]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [MinLength(DescriptionMinLength)]
    [MaxLength(DescriptionMaxLength)]
    public string Description { get; set; } = null!;

    public IEnumerable<AddDestinationTerrainDropDownModel> Terrains { get; set; } = null!;

    public int TerrainId { get; set; }

    public string? ImageUrl { get; set; }

    [Required] public string PublishedOn { get; set; } = null!;
}