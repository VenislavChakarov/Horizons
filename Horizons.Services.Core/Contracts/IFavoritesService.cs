using Horizons.Web.ViewModels.Destination;

namespace Horizons.Services.Core.Contracts;

public interface IFavoritesService
{
    Task<IEnumerable<FavoritesDestinationsViewModel>> GetUserFavoritesAsync(string userId);
    
    Task<bool> IsDestinationInFavoritesAsync(string userId, string destinationId);
    
    Task AddToFavoritesAsync(string userId, string destinationId);
    
    Task RemoveFromFavoritesAsync(string userId, string destinationId);
    
}