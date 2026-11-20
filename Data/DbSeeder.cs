using BeFit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Data;

public static class DbSeeder
{
    public const string RolaAdmin = "Administrator";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        // Dla prostoty baza tworzona jest automatycznie. Można zastąpić migracjami:
        // dotnet ef migrations add Init  oraz  db.Database.MigrateAsync().
        await db.Database.EnsureCreatedAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync(RolaAdmin))
        {
            await roleManager.CreateAsync(new IdentityRole(RolaAdmin));
        }

        var config = services.GetRequiredService<IConfiguration>();
        var email = config["Admin:Email"] ?? "admin@befit.pl";
        var password = config["Admin:Password"] ?? "Admin123!";

        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var admin = await userManager.FindByEmailAsync(email);
        if (admin == null)
        {
            admin = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Nie udało się utworzyć konta administratora: " +
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }
        if (!await userManager.IsInRoleAsync(admin, RolaAdmin))
        {
            await userManager.AddToRoleAsync(admin, RolaAdmin);
        }

        if (!await db.TypyCwiczen.AnyAsync())
        {
            db.TypyCwiczen.AddRange(
                new TypCwiczenia { Nazwa = "Przysiad ze sztangą", Opis = "Ćwiczenie na mięśnie nóg i pośladków." },
                new TypCwiczenia { Nazwa = "Wyciskanie leżąc", Opis = "Ćwiczenie na klatkę piersiową i triceps." },
                new TypCwiczenia { Nazwa = "Martwy ciąg", Opis = "Ćwiczenie na plecy, pośladki i tył ud." },
                new TypCwiczenia { Nazwa = "Wyciskanie nad głowę", Opis = "Ćwiczenie na barki." },
                new TypCwiczenia { Nazwa = "Podciąganie na drążku", Opis = "Ćwiczenie na plecy i biceps." });
            await db.SaveChangesAsync();
        }
    }
}
