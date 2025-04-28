using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contract;
using CodeCart.Core.Specifications;
using CodeCart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CodeCart.Infrastructure.Repositories;

public class GenericRepository<T>(StoreContext _context) : IGenericRepository<T> where T : BaseEntity
{
    public async Task CreateAsync(T entity)
    =>await _context.AddAsync(entity);
    
    public void Delete(T entity)
    => _context.Remove(entity);

    public bool Exists(int id)
    => _context.Set<T>().Any(x => x.Id == id);

    public async Task<IReadOnlyList<T>> GetAllAsync()
    => await _context.Set<T>().ToListAsync();

    public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecefications<T> spec)
    =>await ApplySpecifications(spec).ToListAsync();

    public async Task<T?> GetByIdAsync(int id)
        => await _context.Set<T>().FindAsync(id);

    public async Task<T?> GetEntityWithSpecAsync(ISpecefications<T> spec)
    =>await ApplySpecifications(spec).FirstOrDefaultAsync();

    public async Task<bool> SaveAllAsync()
    => await _context.SaveChangesAsync() > 0;
   

    public void Update(T entity)
    => _context.Update(entity);


    private IQueryable<T> ApplySpecifications(ISpecefications<T> spec)
    => SpecificationsEvaluator<T>.GetQuery(_context.Set<T>() , spec);
    
}
