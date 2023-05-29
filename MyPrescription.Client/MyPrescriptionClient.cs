using System.Net;
using System.Net.Http.Json;
using MyPrescription.Client.Authentication;
using MyPrescription.Common.DTO;
using MyPrescription.Common.Models;

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

    private async Task<List<T>?> GetListAsync<T>(string url) where T : class
    {
        var responseMessage = await ManageUnauthorizedResponseAsync(() => httpClient.GetAsync(url));
        if (responseMessage.IsSuccessStatusCode)
            return await responseMessage.Content.ReadFromJsonAsync<List<T>>();
        return new List<T>();
    }

    public Task<List<PatientDTO>?> GetPatientsAsync() => GetListAsync<PatientDTO>("Patients");

    public Task<List<DoctorDTO>?> GetDoctorsAsync() => GetListAsync<DoctorDTO>("Doctors");

    public Task<List<PrescriptionExpandedDTO>?> GetPrescriptionsByPatientIdAsync(Guid id) => GetListAsync<PrescriptionExpandedDTO>($"Prescriptions/patient/{id}");

    public Task<List<PrescriptionExpandedDTO>?> GetPrescriptionsByCurrentPatientAsync() => GetListAsync<PrescriptionExpandedDTO>($"Prescriptions/current");

    public Task<HttpResponseMessage> GetPatientAsync(string id)
    {
        return ManageUnauthorizedResponseAsync(() => httpClient.GetAsync($"Patients/{id}"));
    }

    public Task<HttpResponseMessage> GetCurrentPatientAsync()
    {
        return ManageUnauthorizedResponseAsync(() => httpClient.GetAsync($"Patients/current"));
    }

    public Task<HttpResponseMessage> CreatePrescriptionAsync(NewPrescription prescription)
    {
        return ManageUnauthorizedResponseAsync(() => httpClient.PostAsJsonAsync($"Prescriptions", prescription));
    }

    public Task<HttpResponseMessage> DeletePrescriptionByIdAsync(Guid prescriptionId)
    {
        return ManageUnauthorizedResponseAsync(() => httpClient.DeleteAsync($"Prescriptions/{prescriptionId}"));
    }

    public Task<HttpResponseMessage> RenewPrescriptionAsync(Guid prescriptionId)
    {
        return ManageUnauthorizedResponseAsync(() => httpClient.PutAsync($"Prescriptions/renew/{prescriptionId}", null));
    }

    public Task<HttpResponseMessage> GetPrescriptionAsync(Guid prescriptionId)
    {
        return ManageUnauthorizedResponseAsync(() => httpClient.GetAsync($"Prescriptions/{prescriptionId}"));
    }
}

