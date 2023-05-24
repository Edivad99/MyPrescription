using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPrescription.Common.DTO;
using MyPrescription.Data.Repository;

namespace MyPrescription.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly DoctorRepository repository;

    public DoctorsController(DoctorRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorsAsync()
    {
        var doctors = await repository.GetDoctorsAsync();
        return StatusCode(StatusCodes.Status200OK, doctors.Select(MapTo));
    }

    private static DoctorDTO MapTo(Data.Entity.User user) => new()
    {
        Id = Guid.Parse(user.Id),
        Name = user.FirstName + " " + user.LastName
    };
}
