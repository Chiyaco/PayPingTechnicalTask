using PayPingTechnicalTask.Entity;
using System.Linq.Expressions;

namespace PayPingTechnicalTask.Data;

public interface IRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAll();

    Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> where);

    Task<T> Get(Guid id);

    Task Insert(T entity);

    Task Update(T entity);

    Task Delete(T entity);

    Task Commit();
}
