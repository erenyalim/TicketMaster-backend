using FluentValidation;
using MediatR;
using TicketMaster.Application.Services;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Entities;

namespace TicketMaster.Application.Banners;

public sealed record BannerCreateCommand(
    string Title,
    string ImageUrl,
    Guid? EventId = null,
    int DisplayOrder = 0,
    bool IsActive = true
) : IRequest<Result<Guid>>;

public sealed class BannerCreateCommandValidator : AbstractValidator<BannerCreateCommand>
{
    public BannerCreateCommandValidator()
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

internal sealed class BannerCreateCommandHandler(
    IBannerRepository bannerRepository,
    IBlobStorageService blobStorageService)
    : IRequestHandler<BannerCreateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        BannerCreateCommand request,
        CancellationToken cancellationToken)
    {
        var banner = new Banner
        {
            Title = request.Title,
            ImageUrl = blobStorageService.NormalizeUrl(request.ImageUrl, "banners"),
            EventId = request.EventId,
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive
        };

        await bannerRepository.AddAsync(banner, cancellationToken);

        return Result<Guid>.Success(banner.Id, "Banner created successfully.");
    }
}
