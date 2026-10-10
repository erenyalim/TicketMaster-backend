using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Common;

namespace TicketMaster.Domain.Events
{
    public sealed class Event : Entity
    {
        private Event()
        {
        }
        public Event(string title, EventType type, DateOnly date,
            TimeOnly time, string description, int capacity, Address address,
            string imageUrl = "")
        {
            Title = title;
            Type = type;
            Date = date;
            Time = time;
            Description = description;
            Capacity = capacity;
            Address = address;
            ImageUrl = imageUrl;
        }

        public string Title { get; private set; } = default!;
        public enum EventType { Konser, Spor, Tiyatro, Sinema, Festival, }
        public EventType Type { get; private set; }
        public DateOnly Date { get; private set; }
        public TimeOnly Time { get; private set; }
        public string Description { get; private set; } = default!;
        public int Capacity { get; private set; }
        public Address Address { get; private set; } = default!;
        public string ImageUrl { get; set; } = string.Empty;

    }

}

