using Calibr8Fit.Api.Interfaces.Model;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;

namespace Calibr8Fit.Api.Services
{
    // Validates user access to entities using table-per-hierarchy inheritance
    public class TPHValidationService<
        TKey,
        TEntity,
        TUserEntity
    >(
        IRepositoryBase<TEntity, TKey> repository,
        IUserRepositoryBase<TUserEntity, TKey> userRepository
    ) : ITPHValidationService<TKey, TEntity, TUserEntity>
        where TEntity : class, IEntity<TKey>
        where TUserEntity : class, IUserEntity<TKey>
        where TKey : notnull
    {
        private readonly IRepositoryBase<TEntity, TKey> _repository = repository;
        private readonly IUserRepositoryBase<TUserEntity, TKey> _userRepository = userRepository;
        public async Task<bool> ValidateUserAccessAsync(string userId, TKey entityId)
        {
            // Check if entity exists
            if (await _repository.KeyExistsAsync(entityId))
                return true;

            // If entity does not exist, check user entity
            if (await _userRepository.UserKeyExistsAsync(userId, entityId))
                return true;

            return false;
        }
    }
}