using Horizons.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Horizons.Data.Configuration;

public class UserDestinationConfiguration : IEntityTypeConfiguration<UserDestination>
{
    public void Configure(EntityTypeBuilder<UserDestination> entity)
    {
         entity
             .HasKey(ud => new { ud.UserId, ud.DestinationId });
         
         entity
             .HasQueryFilter(ud => ud.Destination.IsDeleted == false);

         entity
             .HasOne(ud => ud.User)
             .WithMany()
             .HasForeignKey(ud => ud.UserId)
             .OnDelete(DeleteBehavior.Restrict);

         entity
             .HasOne(ud => ud.Destination)
             .WithMany(d => d.UserDestinations)
             .HasForeignKey(ud => ud.DestinationId)
             .OnDelete(DeleteBehavior.Restrict);
    }
}