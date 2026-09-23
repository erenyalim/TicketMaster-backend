namespace TicketMaster.Domain.Common;

public sealed record Address(
    string Country,
    string City,
    string Location,
    string FullAddress
);