using Calibr8Fit.Api.Interfaces.Model;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;

namespace Calibr8Fit.Api.Services
{
    // Synchronizes client-side entities with the server, handling merges and conflicts
    public class SyncService<T, TKey>(
        IUserSyncRepositoryBase<T, TKey> repository
        ) : ISyncService<T, TKey>
        where T : class, ISyncableUserEntity<TKey>
        where TKey : notnull
    {
        private readonly IUserSyncRepositoryBase<T, TKey> _repository = repository;

        public Task<DateTime> GetLastSyncedAtAsync(string userId)
        {
            // Return the last synced item date for the user
            return _repository.GetLastSyncedAtAsync(userId);
        }
        public async Task<List<T>> Sync(string userId, IEnumerable<T> entities, DateTime lastSyncedAt)
        {
            var result = new List<T>();

            // Retrieve all modified entities since last sync
            var modifiedEntities = await _repository.GetAllFromDateByUserIdAsync(lastSyncedAt, userId);
            // Add new entities to the result

            if (entities.Any())
            {
                // Create a dictionary for quick lookup of entities by their ID
                var entitiesDict = entities.ToDictionary(e => e.Id, e => e);

                // Filter out entities that are already in the database
                foreach (var item in modifiedEntities)
                {
                    if (entitiesDict.TryGetValue(item.Id, out var entity))
                    {
                        if (entity.ModifiedAt < item.ModifiedAt)
                        {
                            entitiesDict.Remove(item.Id);
                            result.Add(entity); // Return newer entity from the database
                        }
                    }
                    else
                        result.Add(item); // Add modified entities from db that are not in the update list
                }

                // If there are no entities to add or update, return the modified entities
                if (entitiesDict.Count == 0) return modifiedEntities;

                // Update existing entities
                var updatedEntities = await _repository.UpdateRangeAsync(entitiesDict.Values);
                // Add updated entities to the result
                result.AddRange(updatedEntities);
                // Remove updated entities from the dictionary
                updatedEntities.ForEach(e => entitiesDict.Remove(e.Id));

                // Add new entities
                var addedEntities = await _repository.AddRangeAsync(entitiesDict.Values);
                // Add added entities to the result
                result.AddRange(addedEntities);

                return result;
            }

            // If no entities to add or update, return entities from the database
            return modifiedEntities;
        }
    }
}