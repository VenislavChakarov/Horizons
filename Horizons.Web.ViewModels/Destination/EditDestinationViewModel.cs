using System.ComponentModel.DataAnnotations;

namespace Horizons.Web.ViewModels.Destination;

public class EditDestinationViewModel : AddDestinationViewModel
{
    
    [Required]
    public string Id { get; set; } = null!;
    
    [Required]
    public string PublisherId { get; set; } = null!;
}