using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyPrescription.Common.Models;
using MyPrescription.Data.Repository;
using MyPrescription.Common.DTO;

namespace MyPrescription.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private static readonly TimeSpan timeTolerance = TimeSpan.FromSeconds(30);

    private readonly AuthRepository repository;
    private readonly TwoFactorAuthenticator twoFactAuth;
    private readonly IConfiguration configuration;

    public AuthController(AuthRepository repository, TwoFactorAuthenticator twoFactAuth, IConfiguration configuration)
    {
        this.repository = repository;
        this.twoFactAuth = twoFactAuth;
        this.configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("registration")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegistrationAsync(RegisterUser user)
    {
        try
        {
            string key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(8)); // 12 characters
            var userDB = new Data.Entity.User()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Gender = user.Gender,
                Birthdate = user.Birthdate.ToDateTime(TimeOnly.MinValue),
                Key2FA = key,
                Role = user.Role,
                DoctorId = user.Role.ToLower() == "patient" ? user.DoctorId : null
            };
            await repository.AddUserAsync(userDB);

            var setupInfo = twoFactAuth.GenerateSetupCode("MyPrescription", userDB.Email, key, false, 3);

            return StatusCode(StatusCodes.Status201Created, new GoogleAuthDTO
            {
                QrCodeImageUrl = setupInfo.QrCodeSetupImageUrl,
                ManualEntrySetupCode = setupInfo.ManualEntryKey
            });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LogInAsync([FromForm(Name = "username"), Required] string username,
                                                [FromForm(Name = "password"), Required] string password,
                                                [FromForm(Name = "twofa_code"), Required] string twofa_code)
    {
        var userDB = await repository.GetUserByEmailAsync(username);
        if (userDB is null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (!BCrypt.Net.BCrypt.Verify(username + password, userDB.Password))
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        if (!twoFactAuth.ValidateTwoFactorPIN(userDB.Key2FA, twofa_code, timeTolerance))
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }

        return StatusCode(StatusCodes.Status200OK, CreateToken(userDB));
    }

    private string CreateToken(Data.Entity.User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenCode = configuration.GetSection("AppSettings:Token").Value!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenCode));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: creds
            );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}
