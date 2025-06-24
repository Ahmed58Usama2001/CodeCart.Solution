using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Infrastructure.Data;
using System.Collections;

namespace CodeCart.Infrastructure.Repositories;

public class UnitOfWork(StoreContext context) : IUnitOfWork
{
    private Hashtable _repositories = new Hashtable();

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity  
    {
        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repository = new GenericRepository<TEntity>(context);
            _repositories.Add(type, repository);
        }

        return _repositories[type] as IGenericRepository<TEntity>;
    }
    public async Task<int> CompleteAsync()
        => await context.SaveChangesAsync();

    public async ValueTask DisposeAsync()
    => await context.DisposeAsync();
}
