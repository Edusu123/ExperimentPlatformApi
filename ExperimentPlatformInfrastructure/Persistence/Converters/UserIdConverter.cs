using ExperimentPlatformDomain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ExperimentPlatformInfrastructure.Persistence.Converters
{
    public class UserIdConverter : ValueConverter<UserId, Guid>
    {
        public UserIdConverter(): base(id => id.Value, value => new UserId(value)) { }
    }
}
