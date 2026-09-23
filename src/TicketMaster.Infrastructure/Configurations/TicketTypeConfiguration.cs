using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketMaster.Domain.Events;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Infrastructure.Configurations;

public sealed class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.TotalQuantity)
            .IsRequired();

        builder.Property(t => t.RemainingQuantity)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CreateUserId)
            .IsRequired();

        builder.HasIndex(t => new
        {
            t.EventId,
            t.Name
        }).IsUnique()
        .HasFilter("[IsDeleted] = 0");

        builder.HasOne<Event>()
                    .WithMany()
                    .HasForeignKey(t => t.EventId)
                    .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_TicketTypes_TotalQuantity_Positive",
                "[TotalQuantity] > 0");

            table.HasCheckConstraint(
                "CK_TicketTypes_RemainingQuantity_Valid",
                "[RemainingQuantity] >= 0 AND [RemainingQuantity] <= [TotalQuantity]");
        });
    }
}