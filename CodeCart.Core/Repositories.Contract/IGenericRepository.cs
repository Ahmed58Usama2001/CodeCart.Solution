using CodeCart.Core.Entities;

namespace CodeCart.Core.Repositories.Contract;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task CreateAsync(T entity);

    void Update(T entity);

    void Delete(T entity);

    Task<IReadOnlyList<T>> GetAllAsync();

    Task<T?> GetByIdAsync(int id);

    Task<bool> SaveAllAsync();

    bool Exists(int id);
}
