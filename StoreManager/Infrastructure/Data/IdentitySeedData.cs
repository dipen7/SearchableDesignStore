using Microsoft.AspNetCore.Identity;
using StoreManager.Constants;
using StoreManager.Models;

namespace StoreManager.Infrastructure.Data
{
    public class IdentitySeedData
    {
        public static async Task Initialize(AppDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
        {

            context.Database.EnsureCreated();


            string password4all = "P@$$w0rd";

            if (await roleManager.FindByNameAsync(UserRoles.Admin) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            if (await roleManager.FindByNameAsync(UserRoles.Member) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Member));
            }

            if (await userManager.FindByNameAsync("aa@aa.aa") == null)
            {
                var user = new AppUser
                {
                    UserName = "aa@aa.aa",
                    Email = "aa@aa.aa",
                    PhoneNumber = "6902341234"
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password4all);
                    await userManager.AddToRoleAsync(user, UserRoles.Admin);
                }
            }

            if (await userManager.FindByNameAsync("mm@mm.mm") == null)
            {
                var user = new AppUser
                {
                    UserName = "mm@mm.mm",
                    Email = "mm@mm.mm",
                    PhoneNumber = "1112223333"
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password4all);
                    await userManager.AddToRoleAsync(user, UserRoles.Member);
                }
            }
        }
    }
}
