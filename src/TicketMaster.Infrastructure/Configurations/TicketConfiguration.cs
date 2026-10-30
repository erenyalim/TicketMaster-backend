using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketMaster.Domain.Orders;
using TicketMaster.Domain.Tickets;
using TicketMaster.Domain.Users;

namespace TicketMaster.Infrastructure.Configurations;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(ticket => ticket.Id);

        builder.Property(ticket => ticket.TicketCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ticket => ticket.QrCodeValue)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ticket => ticket.HolderEmail)
            .HasMaxLength(256);

        builder.Property(ticket => ticket.Status)
            .IsRequired();

        builder.Property(ticket => ticket.CreatedAt)
            .IsRequired();

        builder.Property(ticket => ticket.CreateUserId)
            .IsRequired();

        builder.HasIndex(ticket => ticket.TicketCode)
            .IsUnique();

        builder.HasIndex(ticket => ticket.QrCodeValue)
            .IsUnique();

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(ticket => ticket.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TicketType>()
            .WithMany()
            .HasForeignKey(ticket => ticket.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(ticket => ticket.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
