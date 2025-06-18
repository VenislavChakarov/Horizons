using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels.Destination;
using Microsoft.AspNetCore.Mvc;
using static Horizons.GCommon.EntityConstrains.Destination;

namespace Horizons.Web.Controllers;

public class DestinationController : BaseController
{
    private readonly IDestinationService _destinationService;
    private readonly ITerrainService _terrainService;

    public DestinationController(IDestinationService destinationService, ITerrainService terrainService)
    {
        this._destinationService = destinationService;
        this._terrainService = terrainService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = this.GetUserId();
            var destinations = await _destinationService.GetAllDestinationsAsync(userId);
            return View(destinations);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction(nameof(Index), "Home");
        }
        
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? Id)
    {
        try
        {
            
            var userId = this.GetUserId();

            var destinationDetails = await _destinationService.GetDestinationDetailsAsync(Id, userId);
            if (destinationDetails == null)
            {
                return NotFound();
            }

            return View(destinationDetails);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction(nameof(Index), "Home");
        }
    }

    [HttpGet]

    public async Task<IActionResult> Add()
    {
        
        
        DestinationFromInputViewModel inputModel = new DestinationFromInputViewModel()
        {
            PublishedOn = DateTime.UtcNow.ToString(DateFormating),
            Terrains = await this._terrainService.GetAllTerrainsAsync()
        };
        
        return View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add(DestinationFromInputViewModel inputModel)
    {
        try
        {
            if (!this.ModelState.IsValid)
            {
                return RedirectToAction(nameof(Add));
            }

            bool addResult = await this._destinationService
                .AddDestinationAsync(inputModel, this.GetUserId());

            if (addResult == false)
            {
                ModelState.AddModelError(string.Empty, "Failed to add destination. Please try again.");
                return RedirectToAction(nameof(Add));
            }
            
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction(nameof(Index));
        }
        
    }
}



