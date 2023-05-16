﻿using System.Net;
using System.Net.Http.Json;
using MyPrescription.Client.Authentication;
using MyPrescription.Common.DTO;

namespace MyPrescription.Client;

public class MyPrescriptionClient
{
    private readonly HttpClient httpClient;
    private readonly IAuthenticationService authService;

    public MyPrescriptionClient(HttpClient httpClient, IAuthenticationService authService)
    {
        this.httpClient = httpClient;
        this.authService = authService;
    }

    private async Task<HttpResponseMessage> ManageUnauthorizedResponseAsync(Func<Task<HttpResponseMessage>> func)
    {
        var responseMessage = await func.Invoke();
        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            await authService.LogoutAsync();
        }
        return responseMessage;
    }

    private async Task<List<T>> GetListAsync<T>(string url) where T : class
    {
        var responseMessage = await ManageUnauthorizedResponseAsync(() => httpClient.GetAsync(url));
        if (responseMessage.IsSuccessStatusCode)
            return await responseMessage.Content.ReadFromJsonAsync<List<T>>();
        return null;
    }

    public Task<List<PatientDTO>> GetPatientsAsync() => GetListAsync<PatientDTO>("Patients");

    public Task<HttpResponseMessage> GetPatientAsync(string id)
    {
        return ManageUnauthorizedResponseAsync(() => httpClient.GetAsync($"Patients/{id}"));
    }

    public Task<HttpResponseMessage> PostPatientAsync(MultipartFormDataContent content)
    {
        return ManageUnauthorizedResponseAsync(() => httpClient.PostAsync($"Patients", content));
    }

    /*public Task<HttpResponseMessage> PutPatientAsync(PatientDTO patient)
    {
        return ManageUnauthorizedResponseAsync(() => httpClient.PutAsJsonAsync($"Patients/{patient.Id}", patient));
    }*/

    public Task<HttpResponseMessage> DeletePatientAsync(string id)
    {
        return ManageUnauthorizedResponseAsync(() => httpClient.DeleteAsync($"Patients/{id}"));
    }
}

