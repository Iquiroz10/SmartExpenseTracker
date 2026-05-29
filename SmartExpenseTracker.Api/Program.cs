using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartExpenseTracker.Application.Interfaces;
using SmartExpenseTracker.Application.Services;
using SmartExpenseTracker.Domain.Interfaces;
using SmartExpenseTracker.Infrastructure.Repositories;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Registro de servicios
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();

builder.Build().Run();