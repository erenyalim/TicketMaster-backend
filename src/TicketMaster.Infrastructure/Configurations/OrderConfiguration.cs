using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketMaster.Domain.Orders;
using TicketMaster.Domain.Users;

namespace TicketMaster.Infrastructure.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);

        builder.Property(order => order.GuestFirstName)
            .HasMaxLength(100);

        builder.Property(order => order.GuestLastName)
            .HasMaxLength(100);

        builder.Property(order => order.GuestEmail)
            .HasMaxLength(256);

        builder.Property(order => order.GuestToken)
            .HasMaxLength(128);

        builder.Property(order => order.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(order => order.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(order => order.PaymentReference)
            .HasMaxLength(200);

        builder.Property(order => order.Status)
            .IsRequired();

        builder.Property(order => order.PaymentStatus)
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.CreateUserId)
            .IsRequired();

        builder.Navigation(order => order.Items)
            .AutoInclude();

        builder.HasIndex(order => order.UserId);
        builder.HasIndex(order => order.CartId);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(order => order.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_Orders_TotalAmount_NonNegative",
                "[TotalAmount] >= 0");
        });
    }
}
