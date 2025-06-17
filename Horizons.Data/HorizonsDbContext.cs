using System.Reflection;
using Horizons.Data.Models;

namespace Horizons.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    public class HorizonsDbContext : IdentityDbContext
    {
        public HorizonsDbContext(DbContextOptions<HorizonsDbContext> options)
            : base(options)
        {
        }
        
        public virtual DbSet<Destination> Destinations { get; set; } = null!;
        public virtual DbSet<Terrain> Terrains { get; set; } = null!;
        public virtual DbSet<UserDestination> UserDestinations { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
             builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
