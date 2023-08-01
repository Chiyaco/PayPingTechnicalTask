using Microsoft.EntityFrameworkCore;
using PayPingTechnicalTask.Entity;
using System.Linq.Expressions;

namespace PayPingTechnicalTask.Data;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly TransactionDbContext _context;
    private readonly DbSet<T> _entities;

    public Repository(TransactionDbContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        return await _entities.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> where)
    {
        return await _entities.Where(where).ToListAsync();
    }

    public async Task<T> Get(Guid id)
    {
        return await _entities.SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task Insert(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        await _entities.AddAsync(entity);
    }

    public async Task Update(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        _entities.Update(entity);
    }

    public async Task Delete(T entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        _entities.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }
}
