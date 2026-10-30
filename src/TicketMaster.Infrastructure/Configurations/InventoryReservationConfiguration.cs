using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketMaster.Domain.Orders;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Infrastructure.Configurations;

public sealed class InventoryReservationConfiguration : IEntityTypeConfiguration<InventoryReservation>
{
    public void Configure(EntityTypeBuilder<InventoryReservation> builder)
    {
        builder.HasKey(reservation => reservation.Id);

        builder.Property(reservation => reservation.Quantity)
            .IsRequired();

        builder.Property(reservation => reservation.ExpiresAt)
            .IsRequired();

        builder.Property(reservation => reservation.Status)
            .IsRequired();

        builder.Property(reservation => reservation.CreatedAt)
            .IsRequired();

        builder.Property(reservation => reservation.CreateUserId)
            .IsRequired();

        builder.HasIndex(reservation => new
        {
            reservation.TicketTypeId,
            reservation.Status,
            reservation.ExpiresAt
        });

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(reservation => reservation.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<TicketType>()
            .WithMany()
            .HasForeignKey(reservation => reservation.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_InventoryReservations_Quantity_Positive", "[Quantity] > 0");
        });
    }
}
