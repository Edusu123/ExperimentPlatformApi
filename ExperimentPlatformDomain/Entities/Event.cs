namespace ExperimentPlatformDomain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public Guid ExperimentId { get; set; }
        public string UserId { get; set; } = "";
        public string Type { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
