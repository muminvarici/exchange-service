using System.Linq.Expressions;
using ExchangeService.Core.Entities;
using ExchangeService.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExchangeService.Infrastructure.Data;

public class GenericRepository<TData> : IRepository<TData> where TData : EntityBase
{
    protected readonly DbContext DbContext;
    protected readonly DbSet<TData> Table;
    private IQueryable<TData> _queryable;

    public IQueryable<TData> Queryable
    {
        get => _queryable ?? Table;
        set => _queryable = value;
    }

    public GenericRepository(DbContext dbContext)
    {
        DbContext = dbContext;
        Table = DbContext.Set<TData>();
    }


    public Task<TData> FindAsync(int id, CancellationToken cancellationToken = default)
    {
        return FindAsync(w => w.Id == id, cancellationToken);
    }

    public Task<TData> FindAsync(Expression<Func<TData, bool>> expression, CancellationToken cancellationToken = default)
    {
        return Queryable.FirstOrDefaultAsync(expression, cancellationToken);
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
        var queryable = Queryable.Where(expression);
        return await queryable.ToListAsync(cancellationToken);
    }

    public IRepository<TData> Include<TProperty>(Expression<Func<TData, TProperty>> include)
    {
        Queryable = Queryable.Include(include);
        return this;
    }
}