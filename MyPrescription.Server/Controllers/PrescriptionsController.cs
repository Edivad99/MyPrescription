﻿using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "doctor")]
    public async Task<IActionResult> CreatePrescriptionAsync(NewPrescription prescription)
    {
        var prescriptionDB = new Prescription()
        {
            Id = Guid.NewGuid().ToString(),
            DrugName = prescription.DrugName,
            CreationDate = prescription.CreationDate.ToDateTime(TimeOnly.MinValue),
            IdUser = prescription.PatientId,
            IsFree = prescription.IsFree,
            SingleUseCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(8)),
            IdDoctor = User.GetId().ToString()
        };

        await repository.AddNewPrescriptionAsync(prescriptionDB);
        return StatusCode(StatusCodes.Status201Created);
    }
}
