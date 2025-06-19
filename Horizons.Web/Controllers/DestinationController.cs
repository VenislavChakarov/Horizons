using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels.Destination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Horizons.GCommon.EntityConstrains.Destination;

namespace Horizons.Web.Controllers;

public class DestinationController : BaseController
{
    private readonly IDestinationService _destinationService;
    private readonly ITerrainService _terrainService;
    private readonly IFavoritesService _favoritesService;

    public DestinationController(IDestinationService destinationService, ITerrainService terrainService, IFavoritesService favoritesService)
    {
        this._destinationService = destinationService;
        this._terrainService = terrainService;
        this._favoritesService = favoritesService;
    }

    [AllowAnonymous]
    
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
                return RedirectToAction(nameof(Index));
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
        try
        {
            AddDestinationViewModel inputModel = new AddDestinationViewModel()
            {
                PublishedOn = DateTime.UtcNow.ToString(DateFormating),
                Terrains = await this._terrainService.GetAllTerrainsAsync()
            };

            return View(inputModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction(nameof(Index));
        }

    }

    [HttpPost]
    public async Task<IActionResult> Add(AddDestinationViewModel inputModel)
    {
        if (!ModelState.IsValid)
        {
            return View(inputModel);
        }

        try
        {
            bool addResult = await _destinationService
                .AddDestinationAsync(inputModel, GetUserId());

            if (!addResult)
            {
                ModelState.AddModelError(string.Empty, "Failed to add destination. Please try again.");
                return View(inputModel);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ModelState.AddModelError(string.Empty, "An error occurred while saving the destination.");
            return View(inputModel);
        }

    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        try
        {
            string userId = this.GetUserId()!;
            EditDestinationViewModel? editModel = await _destinationService.GetDestinationForEditAsync(id, userId);
            if (editModel == null)
            {
                return RedirectToAction(nameof(Index));
                ;
            }

            editModel.Terrains = await _terrainService.GetAllTerrainsAsync();

            return View(editModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction(nameof(Index));
        }

    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditDestinationViewModel editModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(editModel);
            }

            bool editResult = await this._destinationService.EditDestinationAsync(editModel, GetUserId());

            if (editResult == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to edit destination. Please try again.");
                return View(editModel);
            }

            return RedirectToAction(nameof(Details), new { id = editModel.Id });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ModelState.AddModelError(string.Empty, "An error occurred while editing the destination.");
            editModel.Terrains = await _terrainService.GetAllTerrainsAsync();
            return View(editModel);
        }

    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        try
        {
            string userId = this.GetUserId()!;
            DeleteDestinationViewModel?
                deleteModel = await _destinationService.GetDestinationForDeleteAsync(id, userId);
            if (deleteModel == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(deleteModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]

    public async Task<IActionResult> Delete(DeleteDestinationViewModel deleteModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(deleteModel);
            }

            bool deleteRes = await this._destinationService.SoftDeleteDestinationAsync(deleteModel, GetUserId());

            if (deleteRes == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete destination. Please try again.");
                return View(deleteModel);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            ModelState.AddModelError(string.Empty, "An error occurred while deleteing the destination.");
            return View(deleteModel);
        }
    }
    
    public async Task<IActionResult> Favorites()
    {
        if (!IsUserAuthenticated())
        {
            return RedirectToAction("Index", "Home");
        }

        var userId = GetUserId();

        var favoritesDestinationsModel = await _favoritesService.GetUserFavoritesAsync(userId);
        
        
        var model = favoritesDestinationsModel.Select(f => new AllDestinationsIndexViewModel
        {
            Id = f.DestinationId,
            Name = f.Name,
            ImageUrl = f.ImageUrl,
            Terrain = f.Terrain
        });


        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddToFavorites(string id)
    {
        try
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = GetUserId();

            bool isInFavorite = await _favoritesService.IsDestinationInFavoritesAsync(userId, id);

            if (!isInFavorite)
            {
                await _favoritesService.AddToFavoritesAsync(userId, id);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction(nameof(Index), "Home");
        }
        
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveFromFavorites(string id)
    {
        try
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = GetUserId();
        

            bool isinFavorites = await _favoritesService.IsDestinationInFavoritesAsync(userId, id);

            if (isinFavorites)
            {
              await _favoritesService.RemoveFromFavoritesAsync(userId, id);
            }

            return RedirectToAction(nameof(Favorites));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction(nameof(Index), "Home");
        }
       
    }
    
}
