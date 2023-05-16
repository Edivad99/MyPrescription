using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPrescription.Common.DTO;
using MyPrescription.Data.Repository;

namespace MyPrescription.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController : ControllerBase
{
    private readonly PatientRepository repository;

    public PatientsController(PatientRepository repository)
    {
        this.repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    //[Authorize(Roles = "doctor")]
    public async Task<IActionResult> GetPatientsAsync()
    {
        var patients = await repository.GetPatientsAsync();
        return StatusCode(StatusCodes.Status200OK, patients.Select(MapTo));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    //[Authorize(Roles = "doctor")]
    public async Task<IActionResult> GetPatientByIdAsync(Guid id)
    {
        var patient = await repository.GetPatientByIdAsync(id.ToString());
        if (patient is null)
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status200OK, MapTo(patient));
    }

    private static PatientDTO MapTo(Data.Entity.User user) => new()
    {
        Id = Guid.Parse(user.Id),
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Birthdate = DateOnly.FromDateTime(user.Birthdate),
        Gender = user.Gender
    };
}
