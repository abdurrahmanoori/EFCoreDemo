using EFCoreDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<MedicineBatch> MedicineBatches => Set<MedicineBatch>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<SaleItemAllocation> SaleItemAllocations => Set<SaleItemAllocation>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<UnitOfMeasure>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Medicine>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Medicine>().HasIndex(x => x.Barcode).IsUnique().HasFilter("[Barcode] IS NOT NULL");
        modelBuilder.Entity<MedicineBatch>().HasIndex(x => new { x.MedicineId, x.BatchNumber }).IsUnique();
        modelBuilder.Entity<Prescription>().HasIndex(x => x.PrescriptionNumber).IsUnique();
        modelBuilder.Entity<Sale>().HasIndex(x => x.InvoiceNumber).IsUnique();
        modelBuilder.Entity<Purchase>().HasIndex(x => x.InvoiceNumber).IsUnique();

        modelBuilder.Entity<Medicine>().Property(x => x.SellingPrice).HasPrecision(18, 2);
        modelBuilder.Entity<MedicineBatch>().Property(x => x.PurchasePrice).HasPrecision(18, 2);
        modelBuilder.Entity<MedicineBatch>().Property(x => x.SellingPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Purchase>().Property(x => x.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseItem>().Property(x => x.UnitCost).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(x => x.Subtotal).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(x => x.Discount).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(x => x.Total).HasPrecision(18, 2);
        modelBuilder.Entity<SaleItem>().Property(x => x.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<SaleItem>().Property(x => x.Discount).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>().Property(x => x.Amount).HasPrecision(18, 2);

        modelBuilder.Entity<Medicine>()
            .HasOne(x => x.Category).WithMany(x => x.Medicines).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Medicine>()
            .HasOne(x => x.UnitOfMeasure).WithMany(x => x.Medicines).HasForeignKey(x => x.UnitOfMeasureId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Medicine>()
            .HasOne(x => x.Manufacturer).WithMany(x => x.Medicines).HasForeignKey(x => x.ManufacturerId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MedicineBatch>()
            .HasOne(x => x.Medicine).WithMany(x => x.Batches).HasForeignKey(x => x.MedicineId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MedicineBatch>()
            .HasOne(x => x.Supplier).WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SaleItemAllocation>()
            .HasOne(x => x.SaleItem).WithMany(x => x.Allocations).HasForeignKey(x => x.SaleItemId);
        modelBuilder.Entity<SaleItemAllocation>()
            .HasOne(x => x.MedicineBatch).WithMany().HasForeignKey(x => x.MedicineBatchId).OnDelete(DeleteBehavior.Restrict);
    }
}
