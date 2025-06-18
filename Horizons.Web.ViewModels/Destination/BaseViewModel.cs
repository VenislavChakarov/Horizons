namespace Horizons.Web.ViewModels.Destination;

public class BaseViewModel
{
    public string Id { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string Terrain { get; set; } = null!;
    
    public string? ImageUrl { get; set; }
    
    public bool IsFavorite { get; set; }
    
    public bool IsPublisher { get; set; }
}