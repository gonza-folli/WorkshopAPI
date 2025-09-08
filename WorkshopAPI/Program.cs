using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using WorkshopAPI.EF;
using WorkshopAPI.FunctionClasses;
using WorkshopAPI.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Azure Key Vault
var keyVaultUrl = builder.Configuration["KeyVault:Url"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());
}

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
}


builder.Services.AddDbContext<MiDBContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DbConnectionString");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        );
    });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IContentValidatorService, ContentValidatorService>();


builder.Services.AddHttpClient("ValidatorFunction", client =>
{
    // En desarrollo: usa appsettings.Development.json, sino Key Vault
    var functionUrl = builder.Environment.IsDevelopment()
        ? builder.Configuration["ValidatorFunction:Url"]  // Desarrollo local
        : builder.Configuration["ValidatorFunctionUrl"];  // Key Vault

    var functionApiKey = builder.Environment.IsDevelopment()
        ? builder.Configuration["ValidatorFunction:ApiKey"]  // Desarrollo local  
        : builder.Configuration["ValidatorFunctionKey"];     // Key Vault

    client.BaseAddress = new Uri(functionUrl);
    client.DefaultRequestHeaders.Add("x-functions-key", functionApiKey);
    client.Timeout = TimeSpan.FromSeconds(30);
});



var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
