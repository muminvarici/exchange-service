using System.Linq.Expressions;
using ExchangeService.Core.Entities;

namespace ExchangeService.Core.Repositories;

public interface IRepository<TData> where TData : EntityBase
{
    IQueryable<TData> Queryable { get; set; }
    IRepository<TData> Include<TProperty>(Expression<Func<TData, TProperty>> include);
    Task<TData> FindAsync(int id, CancellationToken cancellationToken = default);
    Task<TData> FindAsync(Expression<Func<TData, bool>> expression, CancellationToken cancellationToken = default);
    Task<List<TData>> FilterAsync(Expression<Func<TData, bool>> expression, CancellationToken cancellationToken = default);
    Task InsertAsync(TData entity, CancellationToken cancellationToken = default);
    void Update(TData entity);
    void Delete(TData entity);
    Task<bool> SaveAllAsync(CancellationToken cancellationToken = default);
}