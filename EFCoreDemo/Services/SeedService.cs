using EFCoreDemo.Data;
using EFCoreDemo.Domain.Entities;
using EFCoreDemo.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo.Services;

public class SeedService(AppDbContext db, IConfiguration configuration, IHostEnvironment environment)
{
    public async Task SeedAsync()
    {
        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "Analgesics" },
                new Category { Name = "Antibiotics" },
                new Category { Name = "Antihistamines" },
                new Category { Name = "Cardiovascular" });
        }

        if (!await db.UnitsOfMeasure.AnyAsync())
        {
            db.UnitsOfMeasure.AddRange(
                new UnitOfMeasure { Name = "Tablet", Abbreviation = "tab" },
                new UnitOfMeasure { Name = "Capsule", Abbreviation = "cap" },
                new UnitOfMeasure { Name = "Milliliter", Abbreviation = "ml" },
                new UnitOfMeasure { Name = "Vial", Abbreviation = "vial" });
        }

        if (environment.IsDevelopment() && !await db.Users.AnyAsync())
        {
            var username = configuration["BootstrapAdmin:Username"] ?? "admin";
            var password = configuration["BootstrapAdmin:Password"] ?? "ChangeMe123!";
            var admin = new User
            {
                Username = username,
                FullName = "Development Administrator",
                Role = UserRole.Admin,
                IsActive = true
            };
            admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, password);
            db.Users.Add(admin);
        }

        await db.SaveChangesAsync();
    }
}