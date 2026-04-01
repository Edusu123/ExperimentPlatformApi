namespace ExperimentPlatformDomain.Entities
{
    public class Experiment
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsActive { get; set; }

        public List<Variant> Variants { get; set; } = [];
    }
}
