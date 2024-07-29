using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories;
public abstract class BaseRepository<T>(BaseDbContext repositoryContext) : IBaseRepository<T> where T : class
{
    protected BaseDbContext BaseDbContext { get; set; } = repositoryContext;

    public IQueryable<T> Get()
    {
        return this.BaseDbContext.Set<T>().AsNoTracking();
    }

    public IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression)
    {
        return this.BaseDbContext.Set<T>()
            .Where(expression);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return this.BaseDbContext.Database.BeginTransactionAsync();
    }

    public void Create(T entity)
    {
        this.BaseDbContext.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        this.BaseDbContext.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        this.BaseDbContext.Set<T>().Remove(entity);
    }
}