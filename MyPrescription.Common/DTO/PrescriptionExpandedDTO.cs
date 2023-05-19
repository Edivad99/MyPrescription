namespace MyPrescription.Common.DTO;

public class PrescriptionExpandedDTO
{
    public Guid IdUser { get; set; }
    public string SingleUseCode { get; set; }
    public DateOnly CreationDate { get; set; }
    public bool IsFree { get; set; }
    public string DrugName { get; set; }
    public string DoctorName { get; set; }
    public string PharmacistName { get; set; }
}
