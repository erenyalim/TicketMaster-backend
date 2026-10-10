using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketMaster.Domain.Events;

namespace TicketMaster.Infrastructure.Configurations;

public sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.ImageUrl)
            .IsRequired();

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.Date)
            .IsRequired();

        builder.Property(e => e.Time)
            .IsRequired();

        builder.Property(e => e.Capacity)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreateUserId)
            .IsRequired();

        builder.HasIndex(e => new
        {
            e.Title,
            e.Date
        })
        .IsUnique();

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_Events_Capacity_Positive",
                "[Capacity] > 0");
        });

        builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.Location)
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.FullAddress)
                .IsRequired()
                .HasMaxLength(500);
        });
    }
}
