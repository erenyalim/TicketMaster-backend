namespace TicketMaster.Application.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            string? containerName = null,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            string blobName,
            string? containerName = null,
            CancellationToken cancellationToken = default);
    }
}
