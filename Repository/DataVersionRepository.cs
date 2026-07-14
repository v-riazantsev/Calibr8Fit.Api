using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Enums;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Repository.Base;

namespace Calibr8Fit.Api.Repository
{
    // Tracks data version timestamps for each resource type for sync detection
    public class DataVersionRepository(
        ApplicationDbContext context
    ) : RepositoryBase<DataVersion, DataResource>(context), IDataVersionRepository
    {
        public async Task<DataVersion> AddOrUpdateAsync(DataResource dataResource)
        {
            // Update or create version record for resource type
            var dataVersion = await _context.DataVersions.FindAsync(dataResource);

            // Add new data version or update existing
            if (dataVersion is null)
            {
                dataVersion = new DataVersion { DataResource = dataResource };
                await _context.DataVersions.AddAsync(dataVersion);
            }
            else
                dataVersion.LastUpdatedAt = DateTime.UtcNow;

            // Save changes
            await _context.SaveChangesAsync();
            return dataVersion;
        }

        public async Task<DateTime> LastUpdatedAtAsync(DataResource dataResource)
        {
            var dataVersion = await _context.DataVersions.FindAsync(dataResource);

            // If no data version exists, return DateTime.MinValue
            if (dataVersion?.LastUpdatedAt is null)
                return DateTime.MinValue;

            return dataVersion.LastUpdatedAt;
        }
    }
}