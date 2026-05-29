using SmartExpenseTracker.Api.Models;
using SmartExpenseTracker.Api.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace SmartExpenseTracker.Api.Services;

public class ExpenseService : IExpenseService
{
    private readonly ILogger<ExpenseService> _logger;

    // Almacenamiento en memoria — temporal hasta Módulo 3 (SQL Server)
    private static readonly List<Expense> _expenses = new();

    public ExpenseService(ILogger<ExpenseService> logger)
    {
        _logger = logger;
    }

    public Task<IEnumerable<Expense>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all expenses. Count: {Count}", _expenses.Count);
        return Task.FromResult<IEnumerable<Expense>>(_expenses);
    }

    public Task<Expense?> GetByIdAsync(Guid id)
    {
        var expense = _expenses.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(expense);
    }

    public Task<Expense> CreateAsync(Expense expense)
    {
        expense.Id = Guid.NewGuid();
        expense.Date = DateTime.UtcNow;
        _expenses.Add(expense);
        _logger.LogInformation("Expense created with Id: {Id}", expense.Id);
        return Task.FromResult(expense);
    }
}