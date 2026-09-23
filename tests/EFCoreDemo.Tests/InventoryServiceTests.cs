using EFCoreDemo.Contracts;
using EFCoreDemo.Data;
using EFCoreDemo.Domain.Entities;
using EFCoreDemo.Domain.Enums;
using EFCoreDemo.Services;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo.Tests;

public class InventoryServiceTests
{
    static AppDbContext Db() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    static async Task<(AppDbContext Db, Medicine Medicine)> Seed()
    {
        var db = Db();
        var category = new Category { Name = "Pain" };
        var unit = new UnitOfMeasure { Name = "Tablet", Abbreviation = "tab" };
        db.AddRange(category, unit);
        await db.SaveChangesAsync();

        var medicine = new Medicine
        {
            Code = "M1",
            BrandName = "PainAway",
            GenericName = "Paracetamol",
            Strength = "500mg",
            CategoryId = category.Id,
            UnitOfMeasureId = unit.Id,
            DosageForm = DosageForm.Tablet,
            SellingPrice = 10
        };
        db.Add(medicine);
        await db.SaveChangesAsync();
        return (db, medicine);
    }

    [Fact]
    public async Task Fefo_uses_earliest_expiry_and_skips_expired_and_quarantined()
    {
        var (db, medicine) = await Seed();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        db.MedicineBatches.AddRange(
            new MedicineBatch { MedicineId = medicine.Id, BatchNumber = "LATE", ExpiryDate = today.AddMonths(8), QuantityReceived = 20, QuantityAvailable = 20, SellingPrice = 10 },
            new MedicineBatch { MedicineId = medicine.Id, BatchNumber = "EARLY", ExpiryDate = today.AddMonths(2), QuantityReceived = 5, QuantityAvailable = 5, SellingPrice = 10 },
            new MedicineBatch { MedicineId = medicine.Id, BatchNumber = "OLD", ExpiryDate = today.AddDays(-1), QuantityReceived = 50, QuantityAvailable = 50 },
            new MedicineBatch { MedicineId = medicine.Id, BatchNumber = "Q", ExpiryDate = today.AddMonths(1), QuantityReceived = 50, QuantityAvailable = 50, IsQuarantined = true });
        await db.SaveChangesAsync();

        var result = await new InventoryService(db).PreviewFefoAsync(medicine.Id, 8);

        Assert.Equal("EARLY", result[0].BatchNumber);
        Assert.Equal(5, result[0].Quantity);
        Assert.Equal("LATE", result[1].BatchNumber);
        Assert.Equal(3, result[1].Quantity);
    }

    [Fact]
    public async Task Fefo_rejects_insufficient_stock()
    {
        var (db, medicine) = await Seed();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        db.Add(new MedicineBatch { MedicineId = medicine.Id, BatchNumber = "B", ExpiryDate = today.AddMonths(1), QuantityReceived = 2, QuantityAvailable = 2 });
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new InventoryService(db).PreviewFefoAsync(medicine.Id, 3));
    }

    [Fact]
    public async Task Sale_splits_across_batches()
    {
        var (db, medicine) = await Seed();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var first = new MedicineBatch { MedicineId = medicine.Id, BatchNumber = "A", ExpiryDate = today.AddMonths(1), QuantityReceived = 2, QuantityAvailable = 2, SellingPrice = 10 };
        var second = new MedicineBatch { MedicineId = medicine.Id, BatchNumber = "B", ExpiryDate = today.AddMonths(2), QuantityReceived = 10, QuantityAvailable = 10, SellingPrice = 10 };
        db.AddRange(first, second);
        await db.SaveChangesAsync();

        await new InventoryService(db).CreateSaleAsync(
            new CreateSaleRequest(null, null, 0,
                [new SaleItemRequest(medicine.Id, 5)],
                [new PaymentRequest(PaymentMethod.Cash, 50, null)]),
            null);

        Assert.Equal(0, first.QuantityAvailable);
        Assert.Equal(7, second.QuantityAvailable);
        Assert.Equal(2, await db.SaleItemAllocations.CountAsync());
    }

    [Fact]
    public async Task Prescription_required_medicine_cannot_be_sold_without_prescription()
    {
        var (db, medicine) = await Seed();
        medicine.RequiresPrescription = true;
        db.MedicineBatches.Add(new MedicineBatch
        {
            MedicineId = medicine.Id,
            BatchNumber = "RX1",
            ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(3),
            QuantityReceived = 10,
            QuantityAvailable = 10,
            SellingPrice = 10
        });
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new InventoryService(db).CreateSaleAsync(
                new CreateSaleRequest(null, null, 0,
                    [new SaleItemRequest(medicine.Id, 1)],
                    [new PaymentRequest(PaymentMethod.Cash, 10, null)]),
                null));
    }

    [Fact]
    public async Task Stock_adjustment_cannot_go_negative()
    {
        var (db, medicine) = await Seed();
        var batch = new MedicineBatch
        {
            MedicineId = medicine.Id,
            BatchNumber = "ADJ",
            ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(3),
            QuantityReceived = 2,
            QuantityAvailable = 2
        };
        db.Add(batch);
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new InventoryService(db).AdjustStockAsync(
                new StockAdjustmentRequest(batch.Id, -3, StockMovementType.DamagedWriteOff, "damaged"),
                null));
    }
}