using Mapster;
using MediatR;
using TicketMaster.Application.Services;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Entities;

namespace TicketMaster.Application.Banners;

public sealed record BannerGetAllQuery(bool IncludeInactive = false)
    : IRequest<Result<List<BannerGetAllQueryResponse>>>;

public sealed record BannerGetAllQueryResponse(
    Guid Id,
    string Title,
    string ImageUrl,
    Guid? EventId,
    int DisplayOrder,
    bool IsActive
);

internal sealed class BannerGetAllQueryHandler(
    IBannerRepository bannerRepository,
    IBlobStorageService blobStorageService)
    : IRequestHandler<BannerGetAllQuery, Result<List<BannerGetAllQueryResponse>>>
{
    public async Task<Result<List<BannerGetAllQueryResponse>>> Handle(
        BannerGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var banners = await bannerRepository.GetAllAsync(cancellationToken);

        var response = banners
            .Where(banner => request.IncludeInactive || banner.IsActive)
            .OrderBy(banner => banner.DisplayOrder)
            .ThenBy(banner => banner.Title)
            .Adapt<List<BannerGetAllQueryResponse>>();

        for (var i = 0; i < response.Count; i++)
        {
            response[i] = response[i] with
            {
                ImageUrl = blobStorageService.NormalizeUrl(response[i].ImageUrl, "banners")
            };
        }

        return Result<List<BannerGetAllQueryResponse>>.Success(response);
    }
}
