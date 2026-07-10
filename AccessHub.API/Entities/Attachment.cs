namespace AccessHub.API.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }

        public Guid RequestId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string S3Key { get; set; } = string.Empty;
    }
}
