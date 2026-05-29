using SmartExpenseTracker.Domain.Entities;
namespace SmartExpenseTracker.Application.Interfaces;

public interface IExpenseService
{
    Task<IEnumerable<Expense>> GetAllAsync();
    Task<Expense?> GetByIdAsync(Guid id);
    Task<Expense> CreateAsync(Expense expense);
}