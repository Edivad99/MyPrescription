using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyPrescription.Client;
using MyPrescription.Client.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = builder.HostEnvironment.IsDevelopment()
    ? new Uri("https://localhost:7283/")
    : new Uri("https://myprescription.azurewebsites.net/")
});

builder.Services.AddScoped<MyPrescriptionClient>();

await builder.Build().RunAsync();
