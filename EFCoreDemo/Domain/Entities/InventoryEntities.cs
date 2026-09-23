using EFCoreDemo.Domain.Common;
using EFCoreDemo.Domain.Enums;

namespace EFCoreDemo.Domain.Entities;

public class MedicineBatch : BaseEntity
{
    public int MedicineId { get; set; }
    public Medicine Medicine { get; set; } = null!;
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly? ManufactureDate { get; set; }
    public DateOnly ExpiryDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int QuantityReceived { get; set; }
    public int QuantityAvailable { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsQuarantined { get; set; }
    public string? Notes { get; set; }
}

public class StockMovement : BaseEntity
{
    public int MedicineId { get; set; }
    public Medicine Medicine { get; set; } = null!;
    public int? MedicineBatchId { get; set; }
    public MedicineBatch? MedicineBatch { get; set; }
    public StockMovementType Type { get; set; }
    public int Quantity { get; set; }
    public int BalanceAfter { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? Reason { get; set; }
    public int? PerformedByUserId { get; set; }
    public User? PerformedByUser { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}
