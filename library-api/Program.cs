using library_api.Application;
using library_api.Infrastructure;
using library_api.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);


var currentDir = Directory.GetCurrentDirectory();
var envPath = Path.Combine(currentDir, ".env");

if (!File.Exists(envPath))
{
    envPath = Path.Combine(currentDir, "..", "..", ".env");
}

if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($".env file loaded from: {envPath}");
}
else
{
    Console.WriteLine(".env file not found.");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddApplication();   
builder.Services.AddInfrastructure();   

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
