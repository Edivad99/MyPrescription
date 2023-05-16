using MyPrescription.Common.Models;

namespace MyPrescription.Client.Authentication;

public interface IAuthenticationService
{
    Task LoginAsync(AuthenticationUser user);
    Task LogoutAsync();
}
