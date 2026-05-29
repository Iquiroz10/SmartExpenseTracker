using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SmartExpenseTracker.Api.Models;
using SmartExpenseTracker.Api.Services.Interfaces;

namespace SmartExpenseTracker.Api.Functions;

public class ExpensesFunction
{
    private readonly ILogger<ExpensesFunction> _logger;
    private readonly IExpenseService _expenseService;

    public ExpensesFunction(ILogger<ExpensesFunction> logger, IExpenseService expenseService)
    {
        _logger = logger;
        _expenseService = expenseService;
    }

    [Function("GetExpenses")]
    public async Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "expenses")] HttpRequest req)
    {
        _logger.LogInformation("GET /api/expenses called.");
        var expenses = await _expenseService.GetAllAsync();
        return new OkObjectResult(expenses);
    }

    [Function("GetExpenseById")]
    public async Task<IActionResult> GetById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "expenses/{id:guid}")] HttpRequest req,
        Guid id)
    {
        _logger.LogInformation("GET /api/expenses/{Id} called.", id);
        var expense = await _expenseService.GetByIdAsync(id);

        if (expense is null)
            return new NotFoundObjectResult(new { message = $"Expense {id} not found." });

        return new OkObjectResult(expense);
    }

    [Function("CreateExpense")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "expenses")] HttpRequest req)
    {
        _logger.LogInformation("POST /api/expenses called.");

        Expense? expense;

        try
        {
            expense = await JsonSerializer.DeserializeAsync<Expense>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return new BadRequestObjectResult(new { message = "Invalid JSON body." });
        }

        if (expense is null || string.IsNullOrWhiteSpace(expense.Description) || expense.Amount <= 0)
            return new BadRequestObjectResult(new { message = "Description and a positive Amount are required." });

        var created = await _expenseService.CreateAsync(expense);
        return new ObjectResult(created) { StatusCode = 201 };
    }
}