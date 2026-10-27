using MediatR;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Entities;

namespace TicketMaster.Application.Banners;

public sealed record BannerDeleteCommand(Guid Id) : IRequest<Result<string>>;

internal sealed class BannerDeleteCommandHandler(IBannerRepository bannerRepository)
    : IRequestHandler<BannerDeleteCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        BannerDeleteCommand request,
        CancellationToken cancellationToken)
    {
        var banner = await bannerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (banner is null)
        {
            return Result<string>.Failure("No banner was found to delete.");
        }

        await bannerRepository.DeleteAsync(banner, cancellationToken);

        return Result<string>.Success("Banner deleted successfully.");
    }
}
