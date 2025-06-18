using System.ComponentModel;
using System.Globalization;
using Horizons.Data;
using Horizons.Data.Models;
using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels.Destination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Horizons.GCommon.EntityConstrains.Destination;


namespace Horizons.Services.Core;

public class DestinationService : IDestinationService
{
    private readonly HorizonsDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    
public DestinationService(HorizonsDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }
    
    public async Task<IEnumerable<AllDestinationsIndexViewModel>> GetAllDestinationsAsync(string? userId)
    {
        var destinations = await _dbContext.Destinations
            .Include(d => d.Terrain)
            .Include(d => d.UserDestinations)
            .AsNoTracking()
            .Select(d => new AllDestinationsIndexViewModel
            {
                Id = d.Id.ToString(),
                Name = d.Name,
                ImageUrl = d.ImageUrl,
                Terrain = d.Terrain.Name,
                FavoritesCount = d.UserDestinations.Count,
                IsPublisher = String.IsNullOrEmpty(userId) == false
                    ? d.PublisherId.ToLower() == userId!.ToLower()
                    : false,
                IsFavorite = String.IsNullOrEmpty(userId) == false
                    ? d.UserDestinations.Any(ud => ud.UserId.ToLower() == userId!.ToLower())
                    : false,
            })
            .ToListAsync();
        
        return destinations;
    }

    public async Task<DestinationDetailesViewModel> GetDestinationDetailsAsync(int? Id, string? userId)
    {
        DestinationDetailesViewModel? detailsVm = null;
        if (Id.HasValue )
        {
            Destination? destiantionModel = _dbContext.Destinations
                .AsNoTracking()
                .SingleOrDefault(d => d.Id == Id.Value);
            if (destiantionModel != null)
            {
                detailsVm = new DestinationDetailesViewModel
                {
                    Id = destiantionModel.Id.ToString(),
                    Name = destiantionModel.Name,
                    Description = destiantionModel.Description,
                    ImageUrl = destiantionModel.ImageUrl,
                    PublishedOn = destiantionModel.PublishedOn.ToString(DateFormating),
                };
            }
        }

        return detailsVm;

    }

    public async Task<bool> AddDestinationAsync(DestinationFromInputViewModel model, string? userId)
    {
        bool opResult = false;

        IdentityUser user = await _userManager.FindByIdAsync(userId);
        Terrain? terrainRef = await _dbContext
            .Terrains
            .FindAsync(model.TerrainId);
        
        bool IsPublishedOnValid = DateTime.TryParseExact(model.PublishedOn, DateFormating, CultureInfo.InvariantCulture, 
            DateTimeStyles.None, out DateTime publishedOn);
        
        if (user != null && terrainRef != null && IsPublishedOnValid)
        {
            var destination = new Destination
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                PublishedOn = DateTime.UtcNow,
                PublisherId = user.Id,
                TerrainId = model.TerrainId
            };
            
            await _dbContext.Destinations.AddAsync(destination);
            await _dbContext.SaveChangesAsync();
            
            opResult = true;
        }
        
        return opResult;
        
    }
    
}