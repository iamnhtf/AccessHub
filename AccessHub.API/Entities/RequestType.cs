namespace AccessHub.API.Entities
{
    public class RequestType
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<AccessRequest> Requests { get; set; } = [];
    }
}
