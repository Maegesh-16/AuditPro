using Microsoft.EntityFrameworkCore;
using ApprovalService.API.Data;
using ApprovalService.API.HttpClients;
using ApprovalService.API.Repositories;
using ApprovalService.API.Services;

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
builder.Services.AddDbContext<ApprovalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApprovalServiceDb"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Dependency Injection
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<IApprovalService, ApprovalServiceImpl>();

// HTTP Clients
builder.Services.AddHttpClient<IActionServiceClient, ActionServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ActionService"]!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
