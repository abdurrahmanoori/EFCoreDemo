using System.Security.Claims;
using EFCoreDemo.Contracts;
using EFCoreDemo.Data;
using EFCoreDemo.Domain.Entities;
using EFCoreDemo.Domain.Enums;
using EFCoreDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EFCoreDemo.Controllers;

[Authorize][ApiController][Route("api/catalog")]
public class CatalogController(AppDbContext db):ControllerBase
{
    [HttpGet("medicines")] public async Task<ActionResult> Medicines([FromQuery]string? search)
    {
        var q=db.Medicines.AsNoTracking().Include(x=>x.Category).Include(x=>x.UnitOfMeasure).Include(x=>x.Manufacturer).AsQueryable();
        if(!string.IsNullOrWhiteSpace(search))q=q.Where(x=>x.Code.Contains(search)||x.BrandName.Contains(search)||x.GenericName.Contains(search)||(x.Barcode!=null&&x.Barcode.Contains(search)));
        return Ok(await q.OrderBy(x=>x.BrandName).Select(x=>new{x.Id,x.Code,x.BrandName,x.GenericName,x.Strength,x.DosageForm,Category=x.Category.Name,Unit=x.UnitOfMeasure.Abbreviation,Manufacturer=x.Manufacturer==null?null:x.Manufacturer.Name,x.Barcode,x.SellingPrice,x.ReorderLevel,x.RequiresPrescription,x.IsControlled,x.IsActive}).ToListAsync());
    }
    [Authorize(Roles="Admin,Pharmacist")][HttpPost("medicines")] public async Task<ActionResult> Create(MedicineRequest r)
    {
        var m=new Medicine{Code=r.Code.Trim(),BrandName=r.BrandName.Trim(),GenericName=r.GenericName.Trim(),Strength=r.Strength,DosageForm=r.DosageForm,CategoryId=r.CategoryId,UnitOfMeasureId=r.UnitOfMeasureId,ManufacturerId=r.ManufacturerId,Barcode=r.Barcode,SellingPrice=r.SellingPrice,ReorderLevel=r.ReorderLevel,RequiresPrescription=r.RequiresPrescription,IsControlled=r.IsControlled,StorageInstructions=r.StorageInstructions,Description=r.Description};
        db.Medicines.Add(m);await db.SaveChangesAsync();return Created($"/api/catalog/medicines/{m.Id}",m);
    }
    [HttpGet("lookups")] public async Task<ActionResult> Lookups()=>Ok(new{categories=await db.Categories.AsNoTracking().ToListAsync(),units=await db.UnitsOfMeasure.AsNoTracking().ToListAsync(),manufacturers=await db.Manufacturers.AsNoTracking().ToListAsync(),suppliers=await db.Suppliers.AsNoTracking().ToListAsync()});
}

[Authorize][ApiController][Route("api/inventory")]
public class InventoryController(AppDbContext db,InventoryService inventory):ControllerBase
{
    [HttpGet("stock")] public async Task<ActionResult> Stock()
    {
        var today=DateOnly.FromDateTime(DateTime.UtcNow);
        return Ok(await db.Medicines.AsNoTracking().Where(x=>x.IsActive).Select(x=>new{x.Id,x.Code,x.BrandName,x.GenericName,x.ReorderLevel,Available=x.Batches.Where(b=>b.QuantityAvailable>0&&!b.IsQuarantined&&b.ExpiryDate>=today).Sum(b=>b.QuantityAvailable),NextExpiry=x.Batches.Where(b=>b.QuantityAvailable>0&&!b.IsQuarantined&&b.ExpiryDate>=today).Min(b=>(DateOnly?)b.ExpiryDate)}).OrderBy(x=>x.BrandName).ToListAsync());
    }
    [HttpGet("expiring")] public async Task<ActionResult> Expiring(int days=90){var t=DateOnly.FromDateTime(DateTime.UtcNow);var u=t.AddDays(days);return Ok(await db.MedicineBatches.AsNoTracking().Include(x=>x.Medicine).Where(x=>x.QuantityAvailable>0&&x.ExpiryDate>=t&&x.ExpiryDate<=u).OrderBy(x=>x.ExpiryDate).ToListAsync());}
    [HttpGet("expired")] public async Task<ActionResult> Expired(){var t=DateOnly.FromDateTime(DateTime.UtcNow);return Ok(await db.MedicineBatches.AsNoTracking().Include(x=>x.Medicine).Where(x=>x.QuantityAvailable>0&&x.ExpiryDate<t).OrderBy(x=>x.ExpiryDate).ToListAsync());}
    [HttpGet("fefo-preview/{medicineId:int}/{quantity:int}")] public async Task<ActionResult> Fefo(int medicineId,int quantity)=>Ok(await inventory.PreviewFefoAsync(medicineId,quantity));
    [Authorize(Roles="Admin,Pharmacist")][HttpPost("purchases/receive")] public async Task<ActionResult> Receive(ReceivePurchaseRequest r){var p=await inventory.ReceivePurchaseAsync(r,UserId());return Created($"/api/inventory/purchases/{p.Id}",new{p.Id,p.InvoiceNumber,p.TotalAmount});}
    [Authorize(Roles="Admin,Pharmacist")][HttpPost("adjust")] public async Task<ActionResult> Adjust(StockAdjustmentRequest r){var b=await inventory.AdjustStockAsync(r,UserId());return Ok(new{b.Id,b.QuantityAvailable});}
    private int? UserId()=>int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var id)?id:null;
}

[Authorize][ApiController][Route("api/prescriptions")]
public class PrescriptionsController(AppDbContext db):ControllerBase
{
    [HttpGet] public async Task<ActionResult> List()=>Ok(await db.Prescriptions.AsNoTracking().Include(x=>x.Patient).Include(x=>x.Doctor).Include(x=>x.Items).ThenInclude(x=>x.Medicine).OrderByDescending(x=>x.PrescriptionDate).Take(250).ToListAsync());
    [HttpPost] public async Task<ActionResult> Create(CreatePrescriptionRequest r)
    {
        if(r.Items.Count==0||r.Items.Any(x=>x.Quantity<=0))return BadRequest();
        var p=new Prescription{PrescriptionNumber=$"RX-{DateTime.UtcNow:yyyyMMddHHmmssfff}",PatientId=r.PatientId,DoctorId=r.DoctorId,PrescriptionDate=r.PrescriptionDate,ValidUntil=r.ValidUntil,Diagnosis=r.Diagnosis,Notes=r.Notes,Status=PrescriptionStatus.Active,
            Items=r.Items.Select(x=>new PrescriptionItem{MedicineId=x.MedicineId,Dosage=x.Dosage,Frequency=x.Frequency,Duration=x.Duration,QuantityPrescribed=x.Quantity,Instructions=x.Instructions}).ToList()};
        db.Prescriptions.Add(p);await db.SaveChangesAsync();return Created($"/api/prescriptions/{p.Id}",new{p.Id,p.PrescriptionNumber});
    }
}

[Authorize][ApiController][Route("api/sales")]
public class SalesController(AppDbContext db,InventoryService inventory):ControllerBase
{
    [HttpGet] public async Task<ActionResult> List()=>Ok(await db.Sales.AsNoTracking().Include(x=>x.Items).ThenInclude(x=>x.Medicine).Include(x=>x.Payments).OrderByDescending(x=>x.SaleDateUtc).Take(250).ToListAsync());
    [HttpPost] public async Task<ActionResult> Create(CreateSaleRequest r){var uid=int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var id)?id:(int?)null;var s=await inventory.CreateSaleAsync(r,uid);return Created($"/api/sales/{s.Id}",new{s.Id,s.InvoiceNumber,s.Total});}
}

[Authorize][ApiController][Route("api/dashboard")]
public class DashboardController(AppDbContext db):ControllerBase
{
    [HttpGet] public async Task<ActionResult> Get()
    {
        var t=DateOnly.FromDateTime(DateTime.UtcNow);var soon=t.AddDays(90);
        return Ok(new{medicineCount=await db.Medicines.CountAsync(x=>x.IsActive),expiredBatches=await db.MedicineBatches.CountAsync(x=>x.QuantityAvailable>0&&x.ExpiryDate<t),
            expiringBatches=await db.MedicineBatches.CountAsync(x=>x.QuantityAvailable>0&&x.ExpiryDate>=t&&x.ExpiryDate<=soon),
            lowStock=await db.Medicines.CountAsync(x=>x.IsActive&&x.Batches.Where(b=>b.QuantityAvailable>0&&!b.IsQuarantined&&b.ExpiryDate>=t).Sum(b=>b.QuantityAvailable)<=x.ReorderLevel),
            salesToday=await db.Sales.Where(x=>x.SaleDateUtc.Date==DateTime.UtcNow.Date).SumAsync(x=>(decimal?)x.Total)??0});
    }
}