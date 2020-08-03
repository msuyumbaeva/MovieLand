using Microsoft.Extensions.DependencyInjection;
using MovieLand.BLL.Enums;
using MovieLand.BLL.Services;
using MovieLand.Data.ApplicationDbContext;
using MovieLand.Data.Models;
using System;
using System.Threading.Tasks;

namespace MovieLand
{
    public static class DataInitializer
    {
        public static async Task SeedRoles(AppRoleManager roleManager) {
            foreach (var role in Enum.GetNames(typeof(RoleEnum))) {
                if (!await roleManager.RoleExistsAsync(role)) {
                    await roleManager.CreateAsync(new AppRole(role));
                }
            }
        }

        public static async Task SeedUsers(AppUserManager userManager) {
            var admin = new AppUser() { Email = "admin@test.com", UserName = "Admin" };
            var isAdminExists = await userManager.FindByEmailAsync(admin.Email);
            if (isAdminExists == null) {
                var result = await userManager.CreateAsync(admin, "123456");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(admin, RoleEnum.ADMIN.ToString());
                }
            }

            var user = new AppUser() { Email = "johndoe@test.com", UserName = "JohnDoe" };
            var isUserExists = await userManager.FindByEmailAsync(user.Email);
            if (isUserExists == null) {
                var result = await userManager.CreateAsync(user, "123456");
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(user, RoleEnum.USER.ToString());
                }
            }
        }
    }
}
