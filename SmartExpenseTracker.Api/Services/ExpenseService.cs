using Microsoft.Extensions.Logging;
using SmartExpenseTracker.Api.Models;
using SmartExpenseTracker.Api.Services.Interfaces;

namespace SmartExpenseTracker.Api.Services;

public class ExpenseService : IExpenseService
{
    private readonly ILogger<ExpenseService> _logger;
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(ILogger<ExpenseService> logger, IExpenseRepository expenseRepository)
    {
        _logger = logger;
        _expenseRepository = expenseRepository;
    }

    public async Task<IEnumerable<Expense>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all expenses.");
        return await _expenseRepository.GetAllAsync();
    }

    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        return await _expenseRepository.GetByIdAsync(id);
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        return await _expenseRepository.CreateAsync(expense);
    }
}