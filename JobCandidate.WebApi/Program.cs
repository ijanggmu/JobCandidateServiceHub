using JobCandidate.Application.Service;
using JobCandidate.Domain.Entities;
using JobCandidate.Domain.Interfaces;
using JobCandidate.Infrastructure.Caching;
using JobCandidate.Infrastructure.Persistence;
using JobCandidate.Infrastructure.Repositories;
using JobCandidate.Shared.Models;
using JobCandidate.WebApi.Extension;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CandidateDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ICacheRepository<Candidate>, CacheRepository<Candidate>>();

builder.Services.AddScoped<ICandidateService, CandidateService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
});




var app = builder.Build();

await RunDatabaseMigrationAsync(app);

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task RunDatabaseMigrationAsync(WebApplication app)
{

    using var scope = app.Services.CreateScope();

    var context = scope.ServiceProvider.GetRequiredService<CandidateDbContext>();

    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any())
    {
        await context.Database.MigrateAsync();
    }
}
