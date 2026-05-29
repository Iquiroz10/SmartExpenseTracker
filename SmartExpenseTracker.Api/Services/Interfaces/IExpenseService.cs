using SmartExpenseTracker.Api.Models;

namespace SmartExpenseTracker.Api.Services.Interfaces;

public interface IExpenseService
{
    Task<IEnumerable<Expense>> GetAllAsync();
    Task<Expense?> GetByIdAsync(Guid id);
    Task<Expense> CreateAsync(Expense expense);
}