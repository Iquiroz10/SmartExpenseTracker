using SmartExpenseTracker.Domain.Entities;

namespace SmartExpenseTracker.Domain.Interfaces;

public interface IExpenseRepository
{
    Task<IEnumerable<Expense>> GetAllAsync();
    Task<Expense?> GetByIdAsync(Guid id);
    Task<Expense> CreateAsync(Expense expense);
}