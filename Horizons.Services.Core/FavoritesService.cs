using Horizons.Data;
using Horizons.Data.Models;
using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels.Destination;
using Microsoft.EntityFrameworkCore;

namespace Horizons.Services.Core;

public class FavoritesService :IFavoritesService
{
    private readonly HorizonsDbContext _context;
    
    public FavoritesService(HorizonsDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<IEnumerable<FavoritesDestinationsViewModel>> GetUserFavoritesAsync(string userId)
    {
        return await _context.UserDestinations
            .Where(um => um.UserId == userId)
            .Select(um => new FavoritesDestinationsViewModel()
            {
                DestinationId = um.DestinationId.ToString(),
                Name = um.Destination.Name,
                ImageUrl = um.Destination.ImageUrl,
                Terrain = um.Destination.Terrain.Name,
            }).ToListAsync();

    }

    public async Task<bool> IsDestinationInFavoritesAsync(string userId, string destinationId)
    {
        return await _context.UserDestinations
            .AnyAsync(ud => ud.UserId == userId && ud.DestinationId.ToString().ToLower() == destinationId.ToLower());
    }

    public async Task AddToFavoritesAsync(string userId, string destinationId)
    {
       
        var userDestinations = new UserDestination
        {
            UserId = userId,
            DestinationId = int.Parse(destinationId)
        };
        
        await _context.UserDestinations.AddAsync(userDestinations);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromFavoritesAsync(string userId, string destinationId)
    {
        var userDestinations = await _context.UserDestinations
            .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DestinationId.ToString().ToLower() == destinationId.ToLower());

        if (userDestinations != null)
        {
            _context.UserDestinations.Remove(userDestinations);
            await _context.SaveChangesAsync();
        }
    }
}