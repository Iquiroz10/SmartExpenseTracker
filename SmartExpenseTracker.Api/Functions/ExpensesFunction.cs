using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SmartExpenseTracker.Application.Interfaces;
using SmartExpenseTracker.Domain.Entities;

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
        _logger.LogInformation("GetExpenses triggered.");

        var expenses = await _expenseService.GetAllAsync();
        var count = expenses?.Count() ?? 0;

        _logger.LogInformation("GetExpenses returned {Count} records.", count);
        return new OkObjectResult(expenses);
    }

    [Function("GetExpenseById")]
    public async Task<IActionResult> GetById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "expenses/{id:guid}")] HttpRequest req,
        Guid id)
    {
        _logger.LogInformation("GetExpenseById triggered for {ExpenseId}.", id);

        var expense = await _expenseService.GetByIdAsync(id);

        if (expense is null)
        {
            _logger.LogWarning("GetExpenseById: Expense {ExpenseId} not found.", id);
            return new NotFoundObjectResult(new { message = $"Expense {id} not found." });
        }

        _logger.LogInformation("GetExpenseById: Found expense {ExpenseId} — Amount: {Amount}.", id, expense.Amount);
        return new OkObjectResult(expense);
    }

    [Function("CreateExpense")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "expenses")] HttpRequest req)
    {
        _logger.LogInformation("CreateExpense triggered.");

        Expense? expense;

        try
        {
            expense = await JsonSerializer.DeserializeAsync<Expense>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateExpense: Failed to deserialize request body.");
            return new BadRequestObjectResult(new { message = "Invalid JSON body." });
        }

        if (expense is null || string.IsNullOrWhiteSpace(expense.Description) || expense.Amount <= 0)
        {
            _logger.LogWarning("CreateExpense: Validation failed — Description: {Description}, Amount: {Amount}.",
                expense?.Description, expense?.Amount);
            return new BadRequestObjectResult(new { message = "Description and a positive Amount are required." });
        }

        var created = await _expenseService.CreateAsync(expense);

        _logger.LogInformation("CreateExpense: Expense created with Id {ExpenseId}, Amount: {Amount}, Description: {Description}.",
            created.Id, created.Amount, created.Description);

        return new ObjectResult(created) { StatusCode = 201 };
    }
}