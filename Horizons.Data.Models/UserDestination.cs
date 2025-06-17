using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horizons.Data.Models;

public class UserDestination
{
    
    public string UserId { get; set; }
    
    public virtual IdentityUser User { get; set; }
    
    public int DestinationId { get; set; }
    
    public virtual Destination Destination { get; set; }
}
