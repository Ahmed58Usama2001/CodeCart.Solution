using CodeCart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CodeCart.Infrastructure.Data;

public static class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext _context)
    {
        if(_context.Products.Count()==0)
        {
            var procutsData = File.ReadAllText("../CodeCart.Infrastructure/Data/DataSeed/products.json");

            var products = JsonSerializer.Deserialize<List<Product>>(procutsData);

            if(products?.Count()>0)
            { await  _context.AddRangeAsync(products);
                await _context.SaveChangesAsync();
            }
        }
        
        if(_context.DeliveryMethods.Count()==0)
        {
            var dmData = File.ReadAllText("../CodeCart.Infrastructure/Data/DataSeed/delivery.json");

            var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

            if(methods?.Count()>0)
            { await  _context.AddRangeAsync(methods);
                await _context.SaveChangesAsync();
            }
        }
    }


}
