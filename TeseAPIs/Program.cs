using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System.Text;
using TeseAPIs.Data;
using TeseAPIs.Services;
using TeseAPIs.Validations;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.

//AUTH FOR API
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)), // DO NOT GET FROM config! USE AZURE KEYVAULT OR AWS SECRET MANAGER IN PRODUCTION
        ValidateLifetime = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true
    };
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(); // SWAGGER ONLT IN DEV. 

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
new SqliteConnectionFactory(builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddSingleton<ICreditValidations, CreditValidations>();
builder.Services.AddSingleton<IRegistrationValidations, RegistrationValidations>();
builder.Services.AddSingleton<IStudentProgressService, StudentProgressService>();
builder.Services.AddSingleton<IStudentService, StudentService>();
builder.Services.AddSingleton<IRewardService, RewardService>();
builder.Services.AddSingleton<IChallengeService, ChallengeService>();

builder.Services.AddResiliencePipeline("default", x =>
{
    x.AddRetry(new Polly.Retry.RetryStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<Exception>(),
        Delay = TimeSpan.FromSeconds(2),
        MaxRetryAttempts = 2,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081); // Use the container's internal port
});

/*builder.Services.AddCors(options =>
{
    options.AddPolicy("TeseBlazor", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
        .AllowAnyHeader();
    });
});*/


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", builder =>
                builder.WithOrigins("http://172.19.0.4:8080") // URL of your Blazor app
                       .AllowAnyMethod()
                       .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Take out when prod
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("TeseBlazor");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
