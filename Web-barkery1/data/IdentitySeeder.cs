using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_barkery1.Models;

namespace Web_barkery1.data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            IServiceProvider services,
            IConfiguration configuration)
        {
            var roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            // Tạo các vai trò của website
            string[] roles = { "ADMIN", "CUSTOMER" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    CheckResult(
                        await roleManager.CreateAsync(new IdentityRole(role)));
                }
            }

            var email = configuration["SeedOwner:Email"]?.Trim();
            var password = configuration["SeedOwner:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                if (await userManager.GetUsersInRoleAsync("ADMIN") is { Count: > 0 }) return;
                throw new InvalidOperationException("Chưa có chủ quầy. Điền SeedOwner:Email và SeedOwner:Password trong Manage User Secrets.");
            }

            var owner = await userManager.FindByEmailAsync(email);

            if (owner == null)
            {
                owner = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = "Chủ quầy"
                };

                CheckResult(await userManager.CreateAsync(owner, password));
                CheckResult(await userManager.AddToRoleAsync(owner, "ADMIN"));
            }
            else if (!await userManager.IsInRoleAsync(owner, "ADMIN"))
            {
                throw new InvalidOperationException(
                    "Email này đã thuộc tài khoản khác. " +
                    "Hãy chọn email mới để tạo chủ quầy.");
            }

            Console.WriteLine("DA SAN SANG TAI KHOAN CHU QUAY.");
        }

        private static void CheckResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(errors);
            }
        }
    }
}
