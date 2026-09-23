using EFCoreDemo.Contracts;
using EFCoreDemo.Data;
using EFCoreDemo.Domain.Entities;
using EFCoreDemo.Domain.Enums;
using EFCoreDemo.Services;
using Microsoft.EntityFrameworkCore;
namespace EFCoreDemo.Tests;
public class InventoryServiceTests
{
    static AppDbContext Db()=>new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    static async Task<(AppDbContext,Medicine)> Seed()
    {
        var db=Db();var c=new Category{Name="Pain"};var u=new UnitOfMeasure{Name="Tablet",Abbreviation="tab"};db.AddRange(c,u);await db.SaveChangesAsync();
        var m=new Medicine{Code="M1",BrandName="PainAway",GenericName="Paracetamol",Strength="500mg",CategoryId=c.Id,UnitOfMeasureId=u.Id,DosageForm=DosageForm.Tablet,SellingPrice=10};
        db.Add(m);await db.SaveChangesAsync();return(db,m);
    }
    [Fact] public async Task Fefo_uses_earliest_expiry_and_skips_expired_and_quarantined()
    {
        var(db,m)=await Seed();var t=DateOnly.FromDateTime(DateTime.UtcNow);
        db.MedicineBatches.AddRange(
            new MedicineBatch{MedicineId=m.Id,BatchNumber="LATE",ExpiryDate=t.AddMonths(8),QuantityReceived=20,QuantityAvailable=20,SellingPrice=10},
            new MedicineBatch{MedicineId=m.Id,BatchNumber="EARLY",ExpiryDate=t.AddMonths(2),QuantityReceived=5,QuantityAvailable=5,SellingPrice=10},
            new MedicineBatch{MedicineId=m.Id,BatchNumber="OLD",ExpiryDate=t.AddDays(-1),QuantityReceived=50,QuantityAvailable=50},
            new MedicineBatch{MedicineId=m.Id,BatchNumber="Q",ExpiryDate=t.AddMonths(1),QuantityReceived=50,QuantityAvailable=50,IsQuarantined=true});
        await db.SaveChangesAsync();var r=await new InventoryService(db).PreviewFefoAsync(m.Id,8);
        Assert.Equal("EARLY",r[0].BatchNumber);Assert.Equal(5,r[0].Quantity);Assert.Equal("LATE",r[1].BatchNumber);Assert.Equal(3,r[1].Quantity);
    }
    [Fact] public async Task Fefo_rejects_insufficient_stock()
    {
        var(db,m)=await Seed();var t=DateOnly.FromDateTime(DateTime.UtcNow);db.Add(new MedicineBatch{MedicineId=m.Id,BatchNumber="B",ExpiryDate=t.AddMonths(1),QuantityReceived=2,QuantityAvailable=2});await db.SaveChangesAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(()=>new InventoryService(db).PreviewFefoAsync(m.Id,3));
    }
    [Fact] public async Task Sale_splits_across_batches()
    {
        var(db,m)=await Seed();var t=DateOnly.FromDateTime(DateTime.UtcNow);var a=new MedicineBatch{MedicineId=m.Id,BatchNumber="A",ExpiryDate=t.AddMonths(1),QuantityReceived=2,QuantityAvailable=2,SellingPrice=10};var b=new MedicineBatch{MedicineId=m.Id,BatchNumber="B",ExpiryDate=t.AddMonths(2),QuantityReceived=10,QuantityAvailable=10,SellingPrice=10};db.AddRange(a,b);await db.SaveChangesAsync();
        await new InventoryService(db).CreateSaleAsync(new CreateSaleRequest(null,null,0,[new SaleItemRequest(m.Id,5)],[new PaymentRequest(PaymentMethod.Cash,50,null)]),null);
        Assert.Equal(0,a.QuantityAvailable);Assert.Equal(7,b.QuantityAvailable);Assert.Equal(2,await db.SaleItemAllocations.CountAsync());
    }
}