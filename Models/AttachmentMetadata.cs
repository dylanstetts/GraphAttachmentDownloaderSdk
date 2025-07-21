namespace GraphAttachmentDownloaderSdk.Models
{
    public class AttachmentMetadata
    {
        public List<Entry> TopLevelAttachments { get; set; } = new();

        public class Entry
        {
            public string Name { get; set; }
            public string ContentType { get; set; }
            public int Size { get; set; }
            public string ODataType { get; set; }
            public bool IsInline { get; set; }
            public string Level => "top-level";
            public string MessageSubject { get; set; }
            public string MessageId { get; set; }
            public string AttachmentId { get; set; }
            public string MimeSavedAs { get; set; }
        }
    }
}
