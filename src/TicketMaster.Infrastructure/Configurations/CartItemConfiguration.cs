using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketMaster.Domain.Cart;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Infrastructure.Configurations;

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(item => item.CreatedAt)
            .IsRequired();

        builder.Property(item => item.CreateUserId)
            .IsRequired();

        builder.HasIndex(item => new
        {
            item.CartId,
            item.TicketTypeId
        }).IsUnique();

        builder.Navigation(item => item.TicketType)
            .AutoInclude();

        builder.HasOne(item => item.Cart)
            .WithMany(cart => cart.Items)
            .HasForeignKey(item => item.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cartItem => cartItem.TicketType)
            .WithMany()
            .HasForeignKey(item => item.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_CartItems_Quantity_Positive",
                "[Quantity] > 0");

            table.HasCheckConstraint(
                "CK_CartItems_UnitPrice_NonNegative",
                "[UnitPrice] >= 0");
        });
    }
}
