using EFCoreDemo.Domain.Common;
using EFCoreDemo.Domain.Enums;

namespace EFCoreDemo.Domain.Entities;

public class Purchase : BaseEntity
{
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly PurchaseDate { get; set; }
    public PurchaseStatus Status { get; set; } = PurchaseStatus.Draft;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}

public class PurchaseItem : BaseEntity
{
    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;
    public int MedicineId { get; set; }
    public Medicine Medicine { get; set; } = null!;
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly? ManufactureDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal SellingPrice { get; set; }
}
