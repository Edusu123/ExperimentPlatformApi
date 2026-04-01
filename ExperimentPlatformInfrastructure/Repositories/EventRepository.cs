using ExperimentPlatformDomain.Entities;
using ExperimentPlatformDomain.Interfaces;
using ExperimentPlatformInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExperimentPlatformInfrastructure.Repositories
{
    internal class EventRepository(ExperimentDbContext context) : IEventRepository
    {
        private readonly ExperimentDbContext _context = context;

        public async Task AddAsync(Event evt, CancellationToken ct)
        {
            await _context.Events.AddAsync(evt, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct)
            => _context.SaveChangesAsync(ct);
    }
}
