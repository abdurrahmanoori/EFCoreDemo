using EFCoreDemo.Contracts;
using EFCoreDemo.Data;
using EFCoreDemo.Domain.Entities;
using EFCoreDemo.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace EFCoreDemo.Services;
public class InventoryService(AppDbContext db)
{
    public async Task<List<BatchAllocationResult>> PreviewFefoAsync(int medicineId,int quantity,CancellationToken ct=default)
    {
        if(quantity<=0) throw new ArgumentOutOfRangeException(nameof(quantity));
        var today=DateOnly.FromDateTime(DateTime.UtcNow);
        var batches=await db.MedicineBatches.AsNoTracking().Where(x=>x.MedicineId==medicineId&&x.QuantityAvailable>0&&!x.IsQuarantined&&x.ExpiryDate>=today)
            .OrderBy(x=>x.ExpiryDate).ThenBy(x=>x.ReceivedAtUtc).ToListAsync(ct);
        var remaining=quantity; var result=new List<BatchAllocationResult>();
        foreach(var b in batches){if(remaining==0)break;var take=Math.Min(b.QuantityAvailable,remaining);result.Add(new(b.Id,b.BatchNumber,b.ExpiryDate,take,b.SellingPrice));remaining-=take;}
        if(remaining>0) throw new InvalidOperationException($"Insufficient non-expired stock. Missing {remaining} unit(s).");
        return result;
    }

    public async Task<Purchase> ReceivePurchaseAsync(ReceivePurchaseRequest request,int? userId,CancellationToken ct=default)
    {
        if(request.Items.Count==0||request.Items.Any(x=>x.Quantity<=0)) throw new InvalidOperationException("Purchase requires positive-quantity items.");
        IDbContextTransaction? tx=db.Database.IsRelational()?await db.Database.BeginTransactionAsync(ct):null;
        try
        {
            var purchase=new Purchase{SupplierId=request.SupplierId,InvoiceNumber=request.InvoiceNumber.Trim(),PurchaseDate=request.PurchaseDate,Status=PurchaseStatus.Received,Notes=request.Notes,TotalAmount=request.Items.Sum(x=>x.Quantity*x.UnitCost)};
            db.Purchases.Add(purchase); await db.SaveChangesAsync(ct);
            foreach(var i in request.Items)
            {
                if(i.ExpiryDate<=DateOnly.FromDateTime(DateTime.UtcNow)) throw new InvalidOperationException($"Batch {i.BatchNumber} is expired.");
                db.PurchaseItems.Add(new PurchaseItem{PurchaseId=purchase.Id,MedicineId=i.MedicineId,BatchNumber=i.BatchNumber.Trim(),ManufactureDate=i.ManufactureDate,ExpiryDate=i.ExpiryDate,Quantity=i.Quantity,UnitCost=i.UnitCost,SellingPrice=i.SellingPrice});
                var batch=await db.MedicineBatches.SingleOrDefaultAsync(x=>x.MedicineId==i.MedicineId&&x.BatchNumber==i.BatchNumber,ct);
                if(batch is null)
                {
                    batch=new MedicineBatch{MedicineId=i.MedicineId,SupplierId=request.SupplierId,BatchNumber=i.BatchNumber.Trim(),ManufactureDate=i.ManufactureDate,ExpiryDate=i.ExpiryDate,PurchasePrice=i.UnitCost,SellingPrice=i.SellingPrice,QuantityReceived=i.Quantity,QuantityAvailable=i.Quantity};
                    db.MedicineBatches.Add(batch); await db.SaveChangesAsync(ct);
                }
                else
                {
                    if(batch.ExpiryDate!=i.ExpiryDate) throw new InvalidOperationException("Existing batch has another expiry date.");
                    batch.QuantityReceived+=i.Quantity;batch.QuantityAvailable+=i.Quantity;batch.PurchasePrice=i.UnitCost;batch.SellingPrice=i.SellingPrice;
                }
                db.StockMovements.Add(new StockMovement{MedicineId=i.MedicineId,MedicineBatchId=batch.Id,Type=StockMovementType.PurchaseReceipt,Quantity=i.Quantity,BalanceAfter=batch.QuantityAvailable,ReferenceType=nameof(Purchase),ReferenceId=purchase.Id,PerformedByUserId=userId});
            }
            await db.SaveChangesAsync(ct);if(tx is not null)await tx.CommitAsync(ct);return purchase;
        }
        catch{if(tx is not null)await tx.RollbackAsync(ct);throw;}
        finally{if(tx is not null)await tx.DisposeAsync();}
    }

    public async Task<Sale> CreateSaleAsync(CreateSaleRequest request,int? userId,CancellationToken ct=default)
    {
        if(request.Items.Count==0||request.Items.Any(x=>x.Quantity<=0)) throw new InvalidOperationException("Sale requires positive-quantity items.");
        var rx=request.PrescriptionId is null?null:await db.Prescriptions.Include(x=>x.Items).SingleOrDefaultAsync(x=>x.Id==request.PrescriptionId,ct)??throw new InvalidOperationException("Prescription not found.");
        IDbContextTransaction? tx=db.Database.IsRelational()?await db.Database.BeginTransactionAsync(ct):null;
        try
        {
            var sale=new Sale{InvoiceNumber=$"INV-{DateTime.UtcNow:yyyyMMddHHmmssfff}",PatientId=request.PatientId,PrescriptionId=request.PrescriptionId,Discount=request.Discount,SoldByUserId=userId};
            db.Sales.Add(sale);await db.SaveChangesAsync(ct);decimal subtotal=0;
            foreach(var i in request.Items)
            {
                var medicine=await db.Medicines.SingleOrDefaultAsync(x=>x.Id==i.MedicineId&&x.IsActive,ct)??throw new InvalidOperationException("Medicine not found.");
                if(medicine.RequiresPrescription)
                {
                    if(rx is null)throw new InvalidOperationException($"{medicine.BrandName} requires a prescription.");
                    var ri=rx.Items.SingleOrDefault(x=>x.MedicineId==i.MedicineId)??throw new InvalidOperationException("Medicine is not on prescription.");
                    if(ri.QuantityDispensed+i.Quantity>ri.QuantityPrescribed)throw new InvalidOperationException("Dispensing exceeds prescribed quantity.");
                    ri.QuantityDispensed+=i.Quantity;
                }
                var allocations=await PreviewFefoAsync(i.MedicineId,i.Quantity,ct);
                var item=new SaleItem{SaleId=sale.Id,MedicineId=i.MedicineId,Quantity=i.Quantity,UnitPrice=allocations.Sum(x=>x.UnitPrice*x.Quantity)/i.Quantity,Discount=i.Discount};
                db.SaleItems.Add(item);await db.SaveChangesAsync(ct);
                foreach(var a in allocations)
                {
                    var batch=await db.MedicineBatches.SingleAsync(x=>x.Id==a.MedicineBatchId,ct);batch.QuantityAvailable-=a.Quantity;
                    db.SaleItemAllocations.Add(new SaleItemAllocation{SaleItemId=item.Id,MedicineBatchId=batch.Id,Quantity=a.Quantity});
                    db.StockMovements.Add(new StockMovement{MedicineId=i.MedicineId,MedicineBatchId=batch.Id,Type=StockMovementType.Sale,Quantity=-a.Quantity,BalanceAfter=batch.QuantityAvailable,ReferenceType=nameof(Sale),ReferenceId=sale.Id,PerformedByUserId=userId});
                }
                subtotal+=item.UnitPrice*i.Quantity-i.Discount;
            }
            sale.Subtotal=subtotal;sale.Total=Math.Max(0,subtotal-request.Discount);
            if(request.Payments.Sum(x=>x.Amount)<sale.Total)throw new InvalidOperationException("Payments do not cover the sale total.");
            db.Payments.AddRange(request.Payments.Select(p=>new Payment{SaleId=sale.Id,Method=p.Method,Amount=p.Amount,ReferenceNumber=p.ReferenceNumber}));
            if(rx is not null)rx.Status=rx.Items.All(x=>x.QuantityDispensed>=x.QuantityPrescribed)?PrescriptionStatus.Dispensed:PrescriptionStatus.PartiallyDispensed;
            await db.SaveChangesAsync(ct);if(tx is not null)await tx.CommitAsync(ct);return sale;
        }
        catch{if(tx is not null)await tx.RollbackAsync(ct);throw;}
        finally{if(tx is not null)await tx.DisposeAsync();}
    }

    public async Task<MedicineBatch> AdjustStockAsync(StockAdjustmentRequest request,int? userId,CancellationToken ct=default)
    {
        var allowed=new[]{StockMovementType.AdjustmentIn,StockMovementType.AdjustmentOut,StockMovementType.ExpiredWriteOff,StockMovementType.DamagedWriteOff,StockMovementType.SupplierReturn,StockMovementType.SaleReturn};
        if(!allowed.Contains(request.Type))throw new InvalidOperationException("Invalid manual movement type.");
        var batch=await db.MedicineBatches.SingleOrDefaultAsync(x=>x.Id==request.MedicineBatchId,ct)??throw new InvalidOperationException("Batch not found.");
        var balance=batch.QuantityAvailable+request.QuantityChange;if(balance<0)throw new InvalidOperationException("Stock cannot become negative.");
        batch.QuantityAvailable=balance;db.StockMovements.Add(new StockMovement{MedicineId=batch.MedicineId,MedicineBatchId=batch.Id,Type=request.Type,Quantity=request.QuantityChange,BalanceAfter=balance,Reason=request.Reason,PerformedByUserId=userId});
        await db.SaveChangesAsync(ct);return batch;
    }
}