namespace MyPrescription.Data.Entity;

public class PrescriptionExpanded : Prescription
{
    public required string DoctorName { get; set; }
    public required string PharmacistName { get; set; }
}
