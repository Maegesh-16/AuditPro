using Microsoft.EntityFrameworkCore;
using ActionService.API.Data;
using ActionService.API.HttpClients;
using ActionService.API.Repositories;
using ActionService.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database - SQL Server (each microservice owns its own database)
builder.Services.AddDbContext<ActionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ActionServiceDb"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Dependency Injection
builder.Services.AddScoped<ICorrectiveActionRepository, CorrectiveActionRepository>();
builder.Services.AddScoped<ICorrectiveActionService, CorrectiveActionService>();

// HTTP Clients
builder.Services.AddHttpClient<IApprovalServiceClient, ApprovalServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ApprovalService"]!);
});

builder.WebHost.UseUrls($"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}");

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseAuthorization();

app.MapControllers();

app.Run();
