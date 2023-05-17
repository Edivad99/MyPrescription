namespace MyPrescription.Data.Entity;

public class Prescription
{
    public string Id { get; set; }
    public string IdDoctor { get; set; }
    public string IdUser { get; set; }
    public string IdPharmacist { get; set; }
    public string SingleUseCode { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsFree { get; set; }
    public string DrugName { get; set; }
}
