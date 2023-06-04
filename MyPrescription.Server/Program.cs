using System.Text;
using Google.Authenticator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MyPrescription.Data.Repository;
using MyPrescription.Server.Services;
using MyPrescription.Server.Settings;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("MySQL")!;
string jwtToken = builder.Configuration.GetSection("AppSettings:Token").Value!;
var notificationSettings = Configure<NotificationSettings>("NotificationSettings");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insert the Bearer Token",
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.ConfigureSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyPrescription API",
        Version = "v1"
    });
});
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
        builder.SetIsOriginAllowed(o => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtToken)),
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddScoped(_ => new TwoFactorAuthenticator());
builder.Services.AddSingleton(_ => new AuthRepository(connectionString));
builder.Services.AddSingleton(_ => new PatientRepository(connectionString));
builder.Services.AddSingleton(_ => new DoctorRepository(connectionString));
builder.Services.AddSingleton(_ => new PrescriptionRepository(connectionString));
builder.Services.AddSingleton(_ => new NotificationRepository(connectionString));
builder.Services.AddScoped<NotificationService>();

T? Configure<T>(string sectionName) where T : class
{
    var section = builder.Configuration.GetSection(sectionName);
    var settings = section.Get<T>();
    builder.Services.Configure<T>(section);

    return settings;
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

