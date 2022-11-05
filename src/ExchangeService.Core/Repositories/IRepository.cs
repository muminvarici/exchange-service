using System.Linq.Expressions;
using ExchangeService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeService.Core.Repositories;

public interface IRepository<TData> where TData : EntityBase
{
    DbSet<TData> Table { get; }
    Task<List<TData>> FilterAsync(Expression<Func<TData, bool>> expression, CancellationToken cancellationToken = default);
    Task InsertAsync(TData entity, CancellationToken cancellationToken = default);
    void Update(TData entity);
    void Delete(TData entity);
    Task<bool> SaveAllAsync(CancellationToken cancellationToken = default);
}