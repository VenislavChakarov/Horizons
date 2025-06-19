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
        if (Id.HasValue)
        {
            Destination? destinationModel = await _dbContext.Destinations
                .Include(d => d.Terrain)
                .Include(d => d.Publisher)
                .AsNoTracking()
                .SingleOrDefaultAsync(d => d.Id == Id.Value);

            if (destinationModel != null)
            {
                detailsVm = new DestinationDetailesViewModel
                {
                    Id = destinationModel.Id.ToString(),
                    Name = destinationModel.Name,
                    Description = destinationModel.Description,
                    ImageUrl = destinationModel.ImageUrl,
                    PublishedOn = destinationModel.PublishedOn.ToString(DateFormating),
                    Terrain = destinationModel.Terrain.Name,
                    Publisher = destinationModel.Publisher.UserName,
                    IsPublisher = !string.IsNullOrEmpty(userId) && destinationModel.PublisherId.ToLower() == userId.ToLower()
                };
            }
        }

        return detailsVm;
    }

    public async Task<bool> AddDestinationAsync(AddDestinationViewModel? model, string? userId)
    {
        bool opResult = false;

        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        Terrain? terrainRef = await _dbContext
            .Terrains
            .FindAsync(model.TerrainId);
        
        bool IsPublishedOnValid = DateTime.TryParseExact(model.PublishedOn, DateFormating, CultureInfo.InvariantCulture, 
            DateTimeStyles.None, out DateTime publishedOn);
        
        if (user != null && terrainRef != null && IsPublishedOnValid)
        {
            var newDestination = new Destination
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                PublishedOn = publishedOn,
                PublisherId = userId,
                TerrainId = model.TerrainId
            };
            
            await _dbContext.Destinations.AddAsync(newDestination);
            await _dbContext.SaveChangesAsync();
            
            opResult = true;
        }
        
        return opResult;
        
    }

    public async Task<EditDestinationViewModel?> GetDestinationForEditAsync(int? destId, string userId)
    {
        EditDestinationViewModel? editModel = null!;

        if (destId != null) 
        {
            Destination? editDestinationModel =  await this._dbContext
                .Destinations
                .AsNoTracking()
                .SingleOrDefaultAsync(d => d.Id  == destId);

            if ((editDestinationModel != null)
                && (editDestinationModel.PublisherId.ToLower() == userId.ToLower()))
            {
                editModel = new EditDestinationViewModel
                {
                    Id = editDestinationModel.Id.ToString(),
                    Name = editDestinationModel.Name,
                    Description = editDestinationModel.Description,
                    ImageUrl = editDestinationModel.ImageUrl,
                    TerrainId = editDestinationModel.TerrainId,
                    PublishedOn = editDestinationModel.PublishedOn.ToString(DateFormating),
                    PublisherId = editDestinationModel.PublisherId,
                };
            }
        }
        
        return editModel;
    }
    
    public async Task<bool> EditDestinationAsync(EditDestinationViewModel model, string? userId)
    {
        bool opResult = false;

        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        Destination? destinationUpd = await _dbContext
            .Destinations
            .SingleOrDefaultAsync(d => d.Id == int.Parse(model.Id));
        Terrain? terrainRef = await _dbContext
            .Terrains
            .FindAsync(model.TerrainId);

        bool IsPublishedOnValid = DateTime.TryParseExact(model.PublishedOn, DateFormating, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out DateTime publishedOn);

        if (user != null && destinationUpd != null && terrainRef != null && IsPublishedOnValid &&
            destinationUpd.PublisherId.ToLower() == userId.ToLower())
        {
            destinationUpd.Name = model.Name;
            destinationUpd.Description = model.Description;
            destinationUpd.ImageUrl = model.ImageUrl;
            destinationUpd.PublishedOn = DateTime.UtcNow;
            destinationUpd.TerrainId = model.TerrainId;

            await _dbContext.SaveChangesAsync();

            opResult = true;
        }

        return opResult;
    }

    public async Task<DeleteDestinationViewModel?> GetDestinationForDeleteAsync(int? destId, string userId)
    {
         
        DeleteDestinationViewModel? deleteModel = null!;
 
        if (destId != null) 
        {
            Destination? deleteDestination =  await this._dbContext
                .Destinations
                .Include(d => d.Publisher)
                .AsNoTracking()
                .SingleOrDefaultAsync(d => d.Id  == destId);

            if ((deleteDestination != null)
                && (deleteDestination.PublisherId.ToLower() == userId.ToLower()))
            {
                deleteModel = new DeleteDestinationViewModel()
                {
                    Id = deleteDestination.Id,
                    Name = deleteDestination.Name, 
                    Publisher = deleteDestination.Publisher.NormalizedUserName,
                    PublisherId = deleteDestination.PublisherId,
                };
            }
        }
        
        return deleteModel;
    }

    public async  Task<bool> SoftDeleteDestinationAsync(DeleteDestinationViewModel model, string? userId)
    {
        
        bool opResult = false;

        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        Destination? deleteDestination = await _dbContext
            .Destinations
            .SingleOrDefaultAsync(d => d.Id == model.Id); ;

       
        if (user != null  &&
            deleteDestination.PublisherId.ToLower() == userId.ToLower())
        {
            deleteDestination.IsDeleted = true;
            
            await _dbContext.SaveChangesAsync();

            opResult = true;
        }

        return opResult;
    }
    
}