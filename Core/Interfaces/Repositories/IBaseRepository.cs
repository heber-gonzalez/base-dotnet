using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Interfaces.Repositories;

public interface IBaseRepository<T>
    {
        IQueryable<T> Get();
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);
        Task<IDbContextTransaction> BeginTransactionAsync();
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
