namespace MyPrescription.Data.Entity;

public class User
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Gender { get; set; }
    public DateTime Birthdate { get; set; }
    public required string Key2FA { get; set; }
    public required string Role { get; set; }
    public string? DoctorId { get; set; }
}
