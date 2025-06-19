namespace Horizons.Web.ViewModels.Destination;

public class FavoritesDestinationsViewModel 
{
    public string DestinationId { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string Terrain { get; set; } = null!;
    
    public string? ImageUrl { get; set; }
}