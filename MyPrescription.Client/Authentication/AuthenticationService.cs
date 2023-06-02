using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MyPrescription.Common.Models;
using System.Net.Http.Headers;

namespace MyPrescription.Client.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient httpClient;
    private readonly AuthenticationStateProvider authStateProvider;
    private readonly ILocalStorageService localStorage;

    public AuthenticationService(HttpClient httpClient,
                                 AuthenticationStateProvider authStateProvider,
                                 ILocalStorageService localStorage)
    {
        this.httpClient = httpClient;
        this.authStateProvider = authStateProvider;
        this.localStorage = localStorage;
    }

    public async Task LoginAsync(AuthenticationUser user)
    {
        var data = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("username", user.Email),
            new KeyValuePair<string, string>("password", user.Password),
            new KeyValuePair<string, string>("twofa_code", user.TwoFACode)
        });

        var authResult = await httpClient.PostAsync("Auth/login", data);
        authResult.EnsureSuccessStatusCode();

        var accessToken = await authResult.Content.ReadAsStringAsync();

        await localStorage.SetItemAsync("authToken", accessToken);

        ((AuthStateProvider)authStateProvider).NotifyUserAuthentication(accessToken);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
    }

    public async Task LogoutAsync()
    {
        await localStorage.RemoveItemAsync("authToken");

        ((AuthStateProvider)authStateProvider).NotifyUserLogout();

        httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<bool> IsAccessTokenStillValid()
    {
        var accessToken = await localStorage.GetItemAsStringAsync("authToken");
        accessToken = accessToken.Substring(1, accessToken.Length - 2);
        return JwtParser.IsValid(accessToken);
    }
}

