    using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketMaster.Application.Auth;
using TicketMaster.Application.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadsController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;

    public UploadsController(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please select a file to upload.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size cannot exceed 5 MB.");


        using var stream = file.OpenReadStream();
        var imageUrl = await _blobStorageService.UploadAsync(
            fileStream: stream,
            fileName: file.FileName,
            contentType: file.ContentType,
            containerName: "images",
            cancellationToken: cancellationToken);

        return Ok(new { imageUrl });
    }

    [HttpPost("banner")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> UploadBanner([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please select a file to upload.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size cannot exceed 5 MB.");

        using var stream = file.OpenReadStream();
        var imageUrl = await _blobStorageService.UploadAsync(
            fileStream: stream,
            fileName: file.FileName,
            contentType: file.ContentType,
            containerName: "banners",
            cancellationToken: cancellationToken);

        return Ok(new { imageUrl });
    }

    [HttpPost("profile")]
    [Authorize]
    public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please select a file to upload.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size cannot exceed 5 MB.");

        using var stream = file.OpenReadStream();
        var imageUrl = await _blobStorageService.UploadAsync(
            fileStream: stream,
            fileName: file.FileName,
            contentType: file.ContentType,
            containerName: "profiles",
            cancellationToken: cancellationToken);

        return Ok(new { imageUrl });
    }
}