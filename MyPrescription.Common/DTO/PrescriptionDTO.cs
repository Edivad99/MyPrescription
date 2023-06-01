namespace MyPrescription.Common.DTO;

public class PrescriptionDTO
{
    public Guid Id { get; set; }
    public Guid IdUser { get; set; }
    public string SingleUseCode { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsFree { get; set; }
    public string DrugName { get; set; }
}
