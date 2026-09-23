using EFCoreDemo.Data;
using EFCoreDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace EFCoreDemo.Services;
public class SeedService(AppDbContext db)
{
    public async Task SeedAsync()
    {
        if(!await db.Categories.AnyAsync()) db.Categories.AddRange(
            new Category{Name="Analgesics"},new Category{Name="Antibiotics"},new Category{Name="Antihistamines"},new Category{Name="Cardiovascular"});
        if(!await db.UnitsOfMeasure.AnyAsync()) db.UnitsOfMeasure.AddRange(
            new UnitOfMeasure{Name="Tablet",Abbreviation="tab"},new UnitOfMeasure{Name="Capsule",Abbreviation="cap"},
            new UnitOfMeasure{Name="Milliliter",Abbreviation="ml"},new UnitOfMeasure{Name="Vial",Abbreviation="vial"});
        await db.SaveChangesAsync();
    }
}