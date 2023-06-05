using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPrescription.Common.Models;
using MyPrescription.Common.Extensions;
using MyPrescription.Server.Services;

namespace MyPrescription.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService service;

    public NotificationController(NotificationService service)
    {
        this.service = service;
    }

    [AllowAnonymous]
    [HttpGet("publickey")]
    public IActionResult GetPublicKey()
    {
        return StatusCode(StatusCodes.Status200OK, service.GetPublicKey());
    }

    [HttpPost("subscribe")]
    [Authorize(Roles = "patient")]
    public async Task<IActionResult> Subscribe(NotificationSubscription subscription)
    {
        await service.SubscribePatientAsync(User.GetId(), subscription);
        return StatusCode(StatusCodes.Status200OK);
    }
}
