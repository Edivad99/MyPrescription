namespace MyPrescription.Data.Entity;

public class PrescriptionExpanded
{
    public required string Id { get; set; }
    public required string IdDoctor { get; set; }
    public required string IdUser { get; set; }
    public string? IdPharmacist { get; set; }
    public required string SingleUseCode { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsFree { get; set; }
    public required string DrugName { get; set; }
    public required string DoctorName { get; set; }
    public required string PharmacistName { get; set; }
}
