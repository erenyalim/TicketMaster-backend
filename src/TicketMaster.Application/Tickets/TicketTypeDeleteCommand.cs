using MediatR;
using System;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Application.Tickets;

public sealed record TicketTypeDeleteCommand(Guid Id) : IRequest<Result<string>>;

internal sealed class TicketTypeDeleteCommandHandler(ITicketTypeRepository ticketRepository) : IRequestHandler<TicketTypeDeleteCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        TicketTypeDeleteCommand request,
        CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(request.Id, cancellationToken);

        if (ticket is null)
        {
            return Result<string>.Failure("No ticket was found to delete.");
        }

        await ticketRepository.DeleteAsync(ticket, cancellationToken);

        return Result<string>.Success("The ticket has been successfully deleted from the system.");
    }
}
