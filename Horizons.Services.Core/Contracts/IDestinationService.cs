namespace Horizons.Services.Core.Contracts
{
    using Web.ViewModels.Destination;
    public interface IDestinationService
    {
        Task<IEnumerable<AllDestinationsIndexViewModel>> GetAllDestinationsAsync(string? userId) ;
        
        Task <DestinationDetailesViewModel?> GetDestinationDetailsAsync(int? Id, string? userId);
        
        Task<bool> AddDestinationAsync(AddDestinationViewModel model, string? userId);
        
        Task<EditDestinationViewModel?> GetDestinationForEditAsync(int? destId, string userId);
        Task<bool> EditDestinationAsync(EditDestinationViewModel model, string? userId);
        
        Task<DeleteDestinationViewModel?> GetDestinationForDeleteAsync(int? destId, string userId);   
        
        Task<bool> SoftDeleteDestinationAsync(DeleteDestinationViewModel model, string? userId);
        
    }
}
