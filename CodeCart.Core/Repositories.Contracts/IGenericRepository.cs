using CodeCart.Core.Entities;
using CodeCart.Core.Specifications;

namespace CodeCart.Core.Repositories.Contracts;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task CreateAsync(T entity);

    void Update(T entity);

    void Delete(T entity);

    Task<IReadOnlyList<T>> GetAllAsync();

    Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);

    Task<T?> GetByIdAsync(int id);
    Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec);

    Task<bool> SaveAllAsync();

    bool Exists(int id);

    Task<int> GetCountAsync(ISpecification<T> spec);
}
