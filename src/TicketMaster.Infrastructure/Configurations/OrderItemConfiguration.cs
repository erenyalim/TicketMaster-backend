using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketMaster.Domain.Orders;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Infrastructure.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.TicketTypeName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(item => item.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(item => item.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.Property(item => item.CreatedAt)
            .IsRequired();

        builder.Property(item => item.CreateUserId)
            .IsRequired();

        builder.HasOne(item => item.Order)
            .WithMany(order => order.Items)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<TicketType>()
            .WithMany()
            .HasForeignKey(item => item.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_OrderItems_Quantity_Positive", "[Quantity] > 0");
            table.HasCheckConstraint("CK_OrderItems_UnitPrice_NonNegative", "[UnitPrice] >= 0");
            table.HasCheckConstraint("CK_OrderItems_TotalAmount_NonNegative", "[TotalAmount] >= 0");
        });
    }
}
