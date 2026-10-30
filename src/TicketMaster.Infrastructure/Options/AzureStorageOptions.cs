namespace TicketMaster.Infrastructure.Options;

public class AzureStorageOptions
{
    public const string SectionName = "AzureStorage";
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "images";
}
