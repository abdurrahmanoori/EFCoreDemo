using EFCoreDemo.Domain.Common;
using EFCoreDemo.Domain.Enums;

namespace EFCoreDemo.Domain.Entities;

public class Prescription : BaseEntity
{
    public string PrescriptionNumber { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public DateOnly PrescriptionDate { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Active;
    public string? Diagnosis { get; set; }
    public string? Notes { get; set; }
    public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
}

public class PrescriptionItem : BaseEntity
{
    public int PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = null!;
    public int MedicineId { get; set; }
    public Medicine Medicine { get; set; } = null!;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string? Duration { get; set; }
    public int QuantityPrescribed { get; set; }
    public int QuantityDispensed { get; set; }
    public string? Instructions { get; set; }
}
