using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using TicketMaster.Application.Services;
using TicketMaster.Infrastructure.Options;

namespace TicketMaster.Infrastructure.Services;

public class AzureBlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _defaultContainerName;

    public AzureBlobStorageService(IOptions<AzureStorageOptions> options)
    {
        _blobServiceClient = new BlobServiceClient(options.Value.ConnectionString);
        _defaultContainerName = options.Value.ContainerName;
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string? containerName = null, CancellationToken cancellationToken = default)
    {
        var container = containerName ?? _defaultContainerName;
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var extension = Path.GetExtension(fileName);
        var blobName = $"{Guid.NewGuid()}{extension}"; 
        var blobClient = containerClient.GetBlobClient(blobName);

        var blobHttpHeader = new BlobHttpHeaders { ContentType = contentType };

        await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = blobHttpHeader }, cancellationToken);

        return blobClient.Uri.ToString();
    }

    public string NormalizeUrl(string? imageUrl, string? containerName = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return string.Empty;

        if (Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return imageUrl;
        }

        var container = containerName ?? _defaultContainerName;
        var blobName = imageUrl.TrimStart('/');
        return _blobServiceClient.GetBlobContainerClient(container).GetBlobClient(blobName).Uri.ToString();
    }

    public async Task<bool> DeleteAsync(string blobName, string? containerName = null, CancellationToken cancellationToken = default)
    {
        var container = containerName ?? _defaultContainerName;
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);

        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
    }

}