using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Serilog;
using GraphAttachmentDownloaderSdk.Services;
using GraphAttachmentDownloaderSdk.Utils;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            var graphClient = GraphAttachmentDownloaderSdk.Services.GraphClientFactory.Create(config);
            var processor = new AttachmentProcessor(graphClient, config);
            var metadata = await processor.DownloadAttachmentsAsync();

            MetadataExporter.ExportToJson(metadata);
            MetadataExporter.ExportToCsv(metadata);

            Log.Information("Attachment metadata exported.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred.");
        }
    }
}
