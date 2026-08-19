using Cinema.Api;
using Cinema.Domain.Services;
using Cinema.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddCinemaApi();
builder.Services.AddCinemaInfrastructure();
builder.Services.AddScoped<ISeatMapService, SeatMapService>();
builder.Services.AddHealthChecks();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi("v1");
}

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapControllers();
app.MapHealthChecks("/health/live");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}.json");
    app.MapScalarApiReference("/scalar/v1");
}

app.Run();

public partial class Program;
