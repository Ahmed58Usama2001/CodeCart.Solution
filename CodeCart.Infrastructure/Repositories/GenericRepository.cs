using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contracts;
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

    public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
    =>await ApplySpecifications(spec).ToListAsync();

    public async Task<T?> GetByIdAsync(int id)
        => await _context.Set<T>().FindAsync(id);

    public async Task<int> GetCountAsync(ISpecification<T> spec)
    => await ApplySpecifications(spec).CountAsync();

    public async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec)
    =>await ApplySpecifications(spec).FirstOrDefaultAsync();

    public void Update(T entity)
    => _context.Update(entity);


    private IQueryable<T> ApplySpecifications(ISpecification<T> spec)
    => SpecificationsEvaluator<T>.GetQuery(_context.Set<T>() , spec);
    
}
