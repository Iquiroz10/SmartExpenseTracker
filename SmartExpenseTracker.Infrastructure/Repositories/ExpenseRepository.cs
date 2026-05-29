using SmartExpenseTracker.Domain.Entities;
using SmartExpenseTracker.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;


namespace SmartExpenseTracker.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly string _connectionString;
    private readonly ILogger<ExpenseRepository> _logger;

    public ExpenseRepository(IConfiguration configuration, ILogger<ExpenseRepository> logger)
    {
        _connectionString = configuration["SqlConnectionString"]
            ?? throw new InvalidOperationException("SqlConnectionString is not configured.");
        _logger = logger;
    }

    public async Task<IEnumerable<Expense>> GetAllAsync()
    {
        const string sql = "SELECT Id, Description, Amount, Category, Date FROM Expenses";

        using var connection = new SqlConnection(_connectionString);
        _logger.LogInformation("Fetching all expenses from database.");
        return await connection.QueryAsync<Expense>(sql);
    }

    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        const string sql = "SELECT Id, Description, Amount, Category, Date FROM Expenses WHERE Id = @Id";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Expense>(sql, new { Id = id });
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        const string sql = """
            INSERT INTO Expenses (Id, Description, Amount, Category, Date)
            VALUES (@Id, @Description, @Amount, @Category, @Date)
            """;

        expense.Id = Guid.NewGuid();
        expense.Date = DateTime.UtcNow;

        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, expense);

        _logger.LogInformation("Expense created with Id: {Id}", expense.Id);
        return expense;
    }
}