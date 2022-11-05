using System.Linq.Expressions;
using ExchangeService.Core.Entities;
using ExchangeService.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExchangeService.Infrastructure.Data;

public class GenericRepository<TData> : IRepository<TData> where TData :EntityBase
{
    protected readonly DbContext DbContext;
    public DbSet<TData> Table { get; private set; }

    public GenericRepository(DbContext dbContext)
    {
        DbContext = dbContext;
        Table = DbContext.Set<TData>();
    }


    public async Task InsertAsync(TData entity, CancellationToken cancellationToken = default)
    {
        _ = await Table.AddAsync(entity, cancellationToken);
    }

    public void Update(TData entity)
    {
        Table.Update(entity);
    }

    public void Delete(TData entity)
    {
        Table.Remove(entity);
    }

    public async Task<bool> SaveAllAsync(CancellationToken cancellationToken = default)
    {
        int changedRecords = await DbContext.SaveChangesAsync(cancellationToken);
        return changedRecords > 0;
    }

    public async Task<List<TData>> FilterAsync(Expression<Func<TData, bool>> expression, CancellationToken cancellationToken = default)
    {
        var queryable = Table.Where(expression);
        return await Table.ToListAsync(cancellationToken);
    }
 
}