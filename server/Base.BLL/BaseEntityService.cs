using Base.Contracts;
using Base.Contracts.BLL;
using Base.Contracts.DAL;
using Base.Contracts.Domain;

namespace Base.BLL;

public class
    BaseEntityService<TDalEntity, TBllEntity, TRepository> :
        BaseEntityService<TDalEntity, TBllEntity, TRepository, Guid>, IEntityService<TBllEntity>
    where TDalEntity : class, IDomainEntityId
    where TBllEntity : class, IDomainEntityId
    where TRepository : IBaseRepository<TDalEntity>
{
    public BaseEntityService(TRepository repository, IMapper<TDalEntity, TBllEntity> mapper) : base(repository, mapper)
    {
    }
}

public class BaseEntityService<TDalEntity, TBllEntity, TRepository, TKey> : IEntityService<TBllEntity, TKey>
    where TDalEntity : class, IDomainEntityId<TKey>
    where TBllEntity : class, IDomainEntityId<TKey>
    where TRepository : IBaseRepository<TDalEntity, TKey>
    where TKey : struct, IEquatable<TKey>
{
    protected readonly TRepository Repository;
    protected readonly IMapper<TDalEntity, TBllEntity> Mapper;

    public BaseEntityService(TRepository repository , IMapper<TDalEntity, TBllEntity> mapper)
    {
        Repository = repository;
        Mapper = mapper;
    }
    

    public async Task<bool> DeleteAsync(TKey id)
    {
        return await Repository.DeleteAsync(id);
    }
}