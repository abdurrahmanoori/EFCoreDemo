namespace EFCoreDemo.Domain.Enums;

public enum UserRole { Admin, Pharmacist, Cashier, Doctor }
public enum DosageForm { Tablet, Capsule, Syrup, Injection, Cream, Ointment, Drops, Inhaler, Powder, Solution, Suspension, Suppository, Other }
public enum StockMovementType { OpeningStock, PurchaseReceipt, Sale, SaleReturn, SupplierReturn, AdjustmentIn, AdjustmentOut, ExpiredWriteOff, DamagedWriteOff }
public enum PrescriptionStatus { Draft, Active, PartiallyDispensed, Dispensed, Cancelled, Expired }
public enum SaleStatus { Completed, Voided, Refunded }
public enum PaymentMethod { Cash, Card, BankTransfer, MobilePayment, Other }
public enum PurchaseStatus { Draft, Received, Cancelled }
