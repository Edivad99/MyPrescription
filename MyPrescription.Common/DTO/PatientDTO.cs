namespace MyPrescription.Common.DTO;

public class PatientDTO
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public DateOnly Birthdate { get; set; }
}
