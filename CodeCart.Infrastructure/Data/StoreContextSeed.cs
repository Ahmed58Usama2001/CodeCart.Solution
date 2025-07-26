using CodeCart.Core.Entities;
using CodeCart.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CodeCart.Infrastructure.Data;

public static class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext _context, UserManager<AppUser> _userManager)
    {
        if(!await _userManager.Users.AnyAsync(u=>u.UserName=="admin@test.com"))
        {
            var user = new AppUser()
            {
                UserName = "admin@test.com",
                Email = "admin@test.com"
            };

            await _userManager.CreateAsync(user, "P@ssw0rd");
            await _userManager.AddToRoleAsync(user, "Admin");
        }

        if(!await _context.Products.AnyAsync())
        {
            var procutsData = File.ReadAllText("../CodeCart.Infrastructure/Data/DataSeed/products.json");

            var products = JsonSerializer.Deserialize<List<Product>>(procutsData);

            if(products?.Count()>0)
            { await  _context.AddRangeAsync(products);
                await _context.SaveChangesAsync();
            }
        }
        
        if(!await _context.DeliveryMethods.AnyAsync())
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
