using EFCoreDemo.Domain.Common;
using EFCoreDemo.Domain.Enums;

namespace EFCoreDemo.Domain.Entities;

public class Sale : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }
    public int? PrescriptionId { get; set; }
    public Prescription? Prescription { get; set; }
    public DateTime SaleDateUtc { get; set; } = DateTime.UtcNow;
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Completed;
    public int? SoldByUserId { get; set; }
    public User? SoldByUser { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class SaleItem : BaseEntity
{
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public int MedicineId { get; set; }
    public Medicine Medicine { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public ICollection<SaleItemAllocation> Allocations { get; set; } = new List<SaleItemAllocation>();
}

public class SaleItemAllocation : BaseEntity
{
    public int SaleItemId { get; set; }
    public SaleItem SaleItem { get; set; } = null!;
    public int MedicineBatchId { get; set; }
    public MedicineBatch MedicineBatch { get; set; } = null!;
    public int Quantity { get; set; }
}

public class Payment : BaseEntity
{
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime PaidAtUtc { get; set; } = DateTime.UtcNow;
}
