namespace AccessHub.API.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }

        public Guid RequestId { get; set; }

        public AccessRequest Request { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;

        public string S3Key { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }
    }
}
