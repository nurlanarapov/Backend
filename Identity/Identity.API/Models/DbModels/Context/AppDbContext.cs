using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Identity.API.Models.DbModels.Context
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<RefreshTokens> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("auth");
            base.OnModelCreating(builder);
            builder.Entity<AppUser>().HasMany(x => x.RefreshTokens)
                                     .WithOne(s => s.User)
                                     .HasForeignKey(x => x.UserId);
            builder.Entity<IdentityRole>().HasData(new IdentityRole() { Id = Guid.Parse("f2ee6a01-7899-4efd-bff6-6b990355e737").ToString(), Name = "Admin", NormalizedName = "ADMIN" });
        }
    }
}