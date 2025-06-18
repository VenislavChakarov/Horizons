using Horizons.Data;
using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels.Destination;
using Microsoft.EntityFrameworkCore;

namespace Horizons.Services.Core;

public class TerrainService : ITerrainService
{
    private readonly HorizonsDbContext _dbContext;
    
public TerrainService(HorizonsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<AddDestinationTerrainDropDownModel>> GetAllTerrainsAsync()
    {
        var terrainsDropDownMenu = await _dbContext
            .Terrains
            .AsNoTracking()
            .Select(t => new AddDestinationTerrainDropDownModel
            {
                Id = t.Id,
                Name = t.Name
            })
            .ToListAsync();
        
        return terrainsDropDownMenu; 
    }
}