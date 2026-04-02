using ExperimentPlatformDomain.Entities;
using ExperimentPlatformDomain.Interfaces;
using ExperimentPlatformInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExperimentPlatformInfrastructure.Repositories
{
    public class ExperimentRepository(ExperimentDbContext context) : IExperimentRepository
    {
        private readonly ExperimentDbContext _context = context;

        public async Task AddAsync(Experiment experiment, CancellationToken ct)
        {
            await _context.Experiments.AddAsync(experiment, ct);
        }

        public async Task<Experiment?> GetAsync(Guid id, CancellationToken ct)
        {
            return await _context.Experiments
                .Include(x => x.Variants)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public Task UpdateAsync(Experiment experiment, CancellationToken ct)
        {
            _context.Experiments.Update(experiment);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct)
            => _context.SaveChangesAsync(ct);
    }
}
