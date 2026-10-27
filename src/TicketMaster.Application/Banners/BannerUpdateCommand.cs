using FluentValidation;
using MediatR;
using TicketMaster.Application.Services;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Entities;

namespace TicketMaster.Application.Banners;

public sealed record BannerUpdateCommand(
    Guid Id,
    string Title,
    string ImageUrl,
    Guid? EventId = null,
    int DisplayOrder = 0,
    bool IsActive = true
) : IRequest<Result<string>>;

public sealed class BannerUpdateCommandValidator : AbstractValidator<BannerUpdateCommand>
{
    public BannerUpdateCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .MaximumLength(2048);
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}

internal sealed class BannerUpdateCommandHandler(
    IBannerRepository bannerRepository,
    IBlobStorageService blobStorageService)
    : IRequestHandler<BannerUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        BannerUpdateCommand request,
        CancellationToken cancellationToken)
    {
        var banner = await bannerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (banner is null)
        {
            return Result<string>.Failure("No banner was found to update.");
        }

        banner.Title = request.Title;
        banner.ImageUrl = blobStorageService.NormalizeUrl(request.ImageUrl, "banners");
        banner.EventId = request.EventId;
        banner.DisplayOrder = request.DisplayOrder;
        banner.IsActive = request.IsActive;

        await bannerRepository.UpdateAsync(banner, cancellationToken);

        return Result<string>.Success("Banner updated successfully.");
    }
}
