using TeseAPIs.Data;
using TeseAPIs.Services;
using TeseAPIs.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
new SqliteConnectionFactory(builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddSingleton<ICreditValidations, CreditValidations>();
builder.Services.AddSingleton<IRegistrationValidations, RegistrationValidations>();
builder.Services.AddSingleton<IStudentProgressService, StudentProgressService>();
builder.Services.AddSingleton<IStudentService, StudentService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("TeseBlazor", builder =>
    {
        builder.WithOrigins("https://localhost:7103") // Replace with your Blazor app's URL
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("TeseBlazor");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
