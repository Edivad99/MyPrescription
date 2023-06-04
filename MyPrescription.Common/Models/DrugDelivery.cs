using System.ComponentModel.DataAnnotations;

namespace MyPrescription.Common.Models;

public class DrugDeliveryForm
{
    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; }

}