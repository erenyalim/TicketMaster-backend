using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketMaster.Domain.Cart;
using TicketMaster.Domain.Users;

namespace TicketMaster.Infrastructure.Configurations;

public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(cart => cart.Id);

        builder.Property(cart => cart.GuestToken)
            .HasMaxLength(128);

        builder.Property(cart => cart.ExpiresAt)
            .IsRequired();

        builder.Property(cart => cart.CreatedAt)
            .IsRequired();

        builder.Property(cart => cart.CreateUserId)
            .IsRequired();

        builder.Navigation(cart => cart.Items)
            .AutoInclude();

        builder.HasIndex(cart => cart.UserId)
            .IsUnique()
            .HasFilter("[UserId] IS NOT NULL AND [IsDeleted] = 0");

        builder.HasIndex(cart => cart.GuestToken)
            .IsUnique()
            .HasFilter("[GuestToken] IS NOT NULL AND [IsDeleted] = 0");

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(cart => cart.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_Carts_Owner",
                "([UserId] IS NOT NULL AND [GuestToken] IS NULL) OR ([UserId] IS NULL AND [GuestToken] IS NOT NULL)");
        });
    }
}
