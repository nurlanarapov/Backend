using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace Identity.API.Models.DbModels.Initializer
{
    public class ApplicationDbInitializer
    {
        public static void Seed(UserManager<AppUser> userManager)
        {
            if (userManager.FindByNameAsync("admin").Result == null)
            {
                AppUser user = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@domain.com"
                };
                string password = GeneratePassword();
                IdentityResult identityResult = userManager.CreateAsync(user, password).Result;
                userManager.AddToRoleAsync(user, "Admin");
            }
        }

        private static string GeneratePassword()
        {
            var numberOfChars = 6;

            var upperCase = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            var lowerCase = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            var numbers = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var rnd = new Random();

            var total = upperCase
                .Concat(lowerCase)
                .Concat(numbers)
                .ToArray();

            var chars = Enumerable
                .Repeat<int>(0, numberOfChars)
                .Select(i => total[rnd.Next(total.Length)])
                .ToArray();

            return new string(chars);
        }
    }
}
