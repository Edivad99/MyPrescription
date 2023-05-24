using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPrescription.Common.DTO;
using MyPrescription.Common.Extensions;
using MyPrescription.Common.Models;
using MyPrescription.Data.Entity;
using MyPrescription.Data.Repository;

namespace MyPrescription.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly PrescriptionRepository repository;

    public PrescriptionsController(PrescriptionRepository repository)
    {
        this.repository = repository;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize(Roles = "doctor")]
    public async Task<IActionResult> CreatePrescriptionAsync(NewPrescription prescription)
    {
        var prescriptionDB = new Prescription()
        {
            Id = Guid.NewGuid().ToString(),
            DrugName = prescription.DrugName,
            CreationDate = prescription.CreationDate,
            IdUser = prescription.PatientId,
            IsFree = prescription.IsFree,
            SingleUseCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(8)),
            IdDoctor = User.GetId().ToString()
        };

        await repository.AddNewPrescriptionAsync(prescriptionDB);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "doctor")]
    public async Task<IActionResult> GetAllPrescriptionsByPatientIdAsync(Guid patientId)
    {
        var prescriptions = await repository.GetAllPrescriptionsByPatientIdAsync(patientId.ToString());
        if (!prescriptions.Any())
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status200OK, prescriptions.Select(MapTo));
    }

    [HttpGet("current")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "patient")]
    public async Task<IActionResult> GetAllPrescriptionsByCurrentPatientAsync()
    {
        return await GetAllPrescriptionsByPatientIdAsync(User.GetId());
    }

    [HttpDelete("{prescriptionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "doctor")]
    public async Task<IActionResult> DeletePrescriptionByIdAsync(Guid prescriptionId)
    {
        var prescription = await repository.GetPrescriptionByIdAsync(prescriptionId.ToString());
        if (prescription is null)
            return StatusCode(StatusCodes.Status404NotFound);
        if (prescription.IdPharmacist is not null)
            return StatusCode(StatusCodes.Status403Forbidden);
        var result = await repository.DeletePrescriptionByIdAsync(prescriptionId.ToString());
        return StatusCode(result ? StatusCodes.Status200OK : StatusCodes.Status404NotFound);
    }

    [HttpPost("{prescriptionId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "doctor")]
    public async Task<IActionResult> RenewPrescriptionByIdAsync(Guid prescriptionId)
    {
        var prescription = await repository.GetPrescriptionByIdAsync(prescriptionId.ToString());
        if (prescription is null)
            return StatusCode(StatusCodes.Status404NotFound);
        if (prescription.IdPharmacist is not null)
            return StatusCode(StatusCodes.Status403Forbidden);

        var newPrescription = new Prescription()
        {
            Id = Guid.NewGuid().ToString(),
            DrugName = prescription.DrugName,
            CreationDate = DateTime.Now,
            IdUser = prescription.IdUser,
            IsFree = prescription.IsFree,
            IdDoctor = User.GetId().ToString(),
            SingleUseCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(8))
        };

        await repository.AddNewPrescriptionAsync(newPrescription);
        return StatusCode(StatusCodes.Status201Created);
    }

    private static PrescriptionExpandedDTO MapTo(PrescriptionExpanded prescription) => new()
    {
        Id = Guid.Parse(prescription.Id),
        IdUser = Guid.Parse(prescription.IdUser),
        CreationDate = prescription.CreationDate,
        DoctorName = prescription.DoctorName,
        PharmacistName = prescription.PharmacistName,
        DrugName = prescription.DrugName,
        IsFree = prescription.IsFree,
        SingleUseCode = prescription.SingleUseCode
    };
}
