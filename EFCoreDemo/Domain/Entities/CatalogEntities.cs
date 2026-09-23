using EFCoreDemo.Domain.Common;
using EFCoreDemo.Domain.Enums;

namespace EFCoreDemo.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}

public class UnitOfMeasure : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}

public class Manufacturer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}

public class Medicine : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
    public DosageForm DosageForm { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int UnitOfMeasureId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = null!;
    public int? ManufacturerId { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public string? Barcode { get; set; }
    public decimal SellingPrice { get; set; }
    public int ReorderLevel { get; set; }
    public bool RequiresPrescription { get; set; }
    public bool IsControlled { get; set; }
    public string? StorageInstructions { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<MedicineBatch> Batches { get; set; } = new List<MedicineBatch>();
}
