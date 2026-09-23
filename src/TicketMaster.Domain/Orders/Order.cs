using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Abstraction;

namespace TicketMaster.Domain.Orders
{
    public sealed class Order : Entity
    {
        public Order(Guid userId, decimal totalAmount) { 
            UserId = userId;
            TotalAmount = totalAmount;
            Status = OrderStatus.Pending;
        }
       public enum OrderStatus { Pending, Completed, Cancelled, Refunded }
       public OrderStatus Status { get; private set; }
       public decimal TotalAmount { get; private set; } = 0;
       public Guid UserId { get; private set; } 

    }
}
