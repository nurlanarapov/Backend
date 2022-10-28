using Identity.API.Models.DbModels;
using Identity.API.Models.DbModels.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.API.Extensions
{
    public static class IdentitiesExtension
    {
        public static void AddIdentities(this IServiceCollection services)
        {
            services.AddIdentityCore<AppUser>()
                    .AddUserManager<UserManager<AppUser>>()
                    .AddRoles<IdentityRole>()
                    .AddSignInManager<SignInManager<AppUser>>()
                    .AddRoleManager<RoleManager<IdentityRole>>()
                    .AddEntityFrameworkStores<AppDbContext>();
        }
    }
}