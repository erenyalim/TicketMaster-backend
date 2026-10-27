using Mapster;
using MediatR;
using TicketMaster.Application.Services;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Entities;

namespace TicketMaster.Application.Banners;

public sealed record BannerGetByIdQuery(Guid Id)
    : IRequest<Result<BannerGetByIdQueryResponse>>;

public sealed record BannerGetByIdQueryResponse(
    Guid Id,
    string Title,
    string ImageUrl,
    Guid? EventId,
    int DisplayOrder,
    bool IsActive
);

internal sealed class BannerGetByIdQueryHandler(
    IBannerRepository bannerRepository,
    IBlobStorageService blobStorageService)
    : IRequestHandler<BannerGetByIdQuery, Result<BannerGetByIdQueryResponse>>
{
    public async Task<Result<BannerGetByIdQueryResponse>> Handle(
        BannerGetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var banner = await bannerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (banner is null)
        {
            return Result<BannerGetByIdQueryResponse>.Failure("No banner was found.");
        }

        var response = banner.Adapt<BannerGetByIdQueryResponse>() with
        {
            ImageUrl = blobStorageService.NormalizeUrl(banner.ImageUrl, "banners")
        };

        return Result<BannerGetByIdQueryResponse>.Success(response);
    }
}
