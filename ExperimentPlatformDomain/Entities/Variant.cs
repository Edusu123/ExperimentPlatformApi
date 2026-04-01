namespace ExperimentPlatformDomain.Entities
{
    public class Variant
    {
        public Guid Id { get; set; }
        public Guid ExperimentId { get; set; }
        public string Name { get; set; } = "";
        public int Weight { get; set; }
    }
}
