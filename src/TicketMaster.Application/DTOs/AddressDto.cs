using System;
using System.Collections.Generic;
using System.Text;

namespace TicketMaster.Application.DTOs;

    public sealed record AddressDto(
    string Country,
    string City,
    string Location,
    string FullAddress
);

