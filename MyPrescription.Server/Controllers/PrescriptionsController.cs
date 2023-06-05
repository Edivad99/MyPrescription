using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPrescription.Common.DTO;
using MyPrescription.Common.Extensions;
using MyPrescription.Common.Models;
using MyPrescription.Data.Entity;
using MyPrescription.Data.Repository;
using MyPrescription.Server.Services;

namespace MyPrescription.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly PrescriptionRepository repository;
    private readonly NotificationService notification;

    public PrescriptionsController(PrescriptionRepository repository, NotificationService notification)
    {
        this.repository = repository;
        this.notification = notification;
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
            SingleUseCode = Guid.NewGuid().ToString(),
            IdDoctor = User.GetId().ToString()
        };

        await repository.AddNewPrescriptionAsync(prescriptionDB);
        await notification.NotificationNewPrescription(prescription.PatientId);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("patient/{patientId}")]
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

    [HttpPut("renew/{prescriptionId}")]
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
            SingleUseCode = Guid.NewGuid().ToString()
        };

        await repository.AddNewPrescriptionAsync(newPrescription);
        await notification.NotificationNewPrescription(prescription.IdUser);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("current")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "patient")]
    public async Task<IActionResult> GetAllPrescriptionsByCurrentPatientAsync()
    {
        return await GetAllPrescriptionsByPatientIdAsync(User.GetId());
    }

    [HttpGet("{prescriptionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "patient")]
    public async Task<IActionResult> GetPrescriptionsByIdPatientAsync(Guid prescriptionId)
    {
        var prescriptions = await repository.GetAllPrescriptionsByPatientIdAsync(User.GetId().ToString());
        if (!prescriptions.Any())
            return StatusCode(StatusCodes.Status404NotFound);
        var prescription = prescriptions.Single(x => x.Id == prescriptionId.ToString());
        if (prescription is null)
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status200OK, MapTo(prescription));
    }

    [HttpPut("deliver/{prescriptionCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "pharmacist")]
    public async Task<IActionResult> DeliverPrescriptionByIdAsync(string prescriptionCode)
    {
        var prescription = await repository.GetPrescriptionByCode(prescriptionCode);
        if (prescription is null)
            return StatusCode(StatusCodes.Status404NotFound);
        if (prescription.IdPharmacist is not null)
            return StatusCode(StatusCodes.Status403Forbidden);

        var result = await repository.MarkPrescriptionAsDeliveredsync(prescription.Id.ToString(), User.GetId().ToString());
        if (result)
            await notification.NotificationPrescriptionDelivered(prescription.IdUser);
        return StatusCode(result ? StatusCodes.Status200OK : StatusCodes.Status404NotFound);
    }

    [HttpGet("singleusecode/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "pharmacist")]
    public async Task<IActionResult> GetPrescriptionsByCode(string code)
    {
        var prescription = await repository.GetPrescriptionByCode(code);
        if (prescription is null)
            return StatusCode(StatusCodes.Status404NotFound);
        return StatusCode(StatusCodes.Status200OK, MapTo(prescription));
    }

    private static PrescriptionDTO MapTo(Prescription prescription) => new()
    {
        Id = Guid.Parse(prescription.Id),
        IdUser = Guid.Parse(prescription.IdUser),
        CreationDate = prescription.CreationDate,
        DrugName = prescription.DrugName,
        IsFree = prescription.IsFree,
        SingleUseCode = prescription.SingleUseCode
    };

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
