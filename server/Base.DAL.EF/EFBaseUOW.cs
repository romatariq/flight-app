using Base.Contracts.DAL;
using Microsoft.EntityFrameworkCore;

namespace Base.DAL.EF;

public class EFBaseUOW<TDbContext> : IBaseUOW
    where TDbContext : DbContext
{
    protected readonly TDbContext DbContext;

    public EFBaseUOW(TDbContext context)
    {
        DbContext = context;
    }
    
    public virtual async Task<int> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }
}
