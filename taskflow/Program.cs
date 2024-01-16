using System.Text;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using taskflow.Data;
using taskflow.Mappings;
using taskflow.Middlewares;
using taskflow.Models.Domain;
using taskflow.Repositories.Implementations;
using taskflow.Repositories.Interfaces;
using taskflow.Services.Impls;
using taskflow.Services.Interfaces;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .WithMethods("GET", "POST", "PATCH", "PUT", "DELETE")
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials();
        });
});
builder.Services.AddEndpointsApiExplorer();

// Inject Serilog for error Logging
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/TaskFlow_Log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Warning()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


builder.Services.AddControllers();

// Inject TaskFlowDbContext into the app - production


// Inject Repositories
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IWorkspaceRepository, WorskpaceRepository>();
builder.Services.AddScoped<IWorkspaceMemberRepository, WorkspaceMemberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// Inject services
builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
builder.Services.AddScoped<IEmailService, EmailService>();


// Inject AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Add Identity packages/solutions
builder.Services.AddIdentityCore<User>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>("TaskFlow")
    .AddEntityFrameworkStores<TaskFlowDbContext>()
    .AddDefaultTokenProviders();

// Setup Identity Options.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Add authentication to the services as well
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException()))
        });

if (builder.Environment.IsProduction())
{
    var keyVaultURL = builder.Configuration.GetSection("KeyVault: KeyVaultURL");

    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));

    builder.Configuration.AddAzureKeyVault(keyVaultURL.Value!, new DefaultKeyVaultSecretManager());

    var client = new SecretClient(new Uri(keyVaultURL.Value!), new DefaultAzureCredential());

    builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    {
        options.UseSqlServer(client.GetSecret("ConnectionStrings--TaskFlowConnectionString").Value.Value);
    });
}

// Inject TaskFlowDbContext into the app - local
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<TaskFlowDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("TaskFlowConnectionString")));
}


builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();


// Inject global exception handler
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<GlobalJsonRequestFormatRequirementMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();

app.MapControllers();

app.Run();