namespace Horizons.Services.Core.Contracts
{
    using Web.ViewModels.Destination;
    public interface IDestinationService
    {
        Task<IEnumerable<AllDestinationsIndexViewModel>> GetAllDestinationsAsync(string? userId) ;
        
        Task <DestinationDetailesViewModel> GetDestinationDetailsAsync(int? Id, string? userId);
        
        Task<bool> AddDestinationAsync(DestinationFromInputViewModel model, string? userId);
        
    }
}
