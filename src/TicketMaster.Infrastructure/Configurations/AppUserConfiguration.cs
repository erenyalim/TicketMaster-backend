using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketMaster.Domain.Users;

namespace TicketMaster.Infrastructure.Configurations;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.FirstName)
                .HasMaxLength(50);
        builder.Property(u => u.LastName)
                .HasMaxLength(50);
                            

        builder.Ignore(u => u.FullName);
    }
}
