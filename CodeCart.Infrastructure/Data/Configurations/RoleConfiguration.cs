using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeCart.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
            builder.HasData(
                new IdentityRole() { Id = "b7a7bc4f-d8ff-45c2-8d91-3f2bc1c2e123", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole() { Id = "e2a0f3fc-ae47-4b9f-89aa-5870f26bd823", Name = "Customer", NormalizedName = "CUSTOMER" }
             );



    }
}
