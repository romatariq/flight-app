using Base.Contracts.DAL;
using Base.Domain;
using Base.Contracts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Base.DAL.EF;

public class EFBaseRepository<TEntity, TDbContext> : EFBaseRepository<TEntity, Guid, TDbContext>, IBaseRepository<TEntity>
    where TEntity: class, IDomainEntityId
    where TDbContext: DbContext
{
    public EFBaseRepository(TDbContext dbContext) : base(dbContext)
    {
    }
}

public class EFBaseRepository<TEntity, TKey, TDbContext> : IBaseRepository<TEntity, TKey>
    where TEntity : class, IDomainEntityId<TKey>
    where TKey: struct, IEquatable<TKey>
    where TDbContext : DbContext
{
    
    protected TDbContext DbContext;
    protected DbSet<TEntity> DbSet;
    
    public EFBaseRepository(TDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        DbSet = dbContext.Set<TEntity>();
    }
    
    

    public virtual async Task<bool> DeleteAsync(TKey id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null) return false;
        
        DbSet.Remove(entity);
        return true;
    }
}