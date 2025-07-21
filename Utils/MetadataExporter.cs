using CsvHelper;
using GraphAttachmentDownloaderSdk.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace GraphAttachmentDownloaderSdk.Utils
{
    public static class MetadataExporter
    {
        public static void ExportToJson(AttachmentMetadata metadata)
        {
            File.WriteAllText("attachment_metadata.json", JsonConvert.SerializeObject(metadata, Formatting.Indented));
        }

        public static void ExportToCsv(AttachmentMetadata metadata)
        {
            using var writer = new StreamWriter("attachment_metadata.csv");
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(metadata.TopLevelAttachments);
        }
    }
}
