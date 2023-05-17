using System.ComponentModel.DataAnnotations;

namespace MyPrescription.Common.Models;

public class NewPrescription
{
    [Required]
    public string DrugName { get; set; }
    [Required]
    public DateOnly CreationDate { get; set; }
    [Required]
    public string PatientId { get; set; }
    [Required]
    public bool IsFree { get; set; }
}

