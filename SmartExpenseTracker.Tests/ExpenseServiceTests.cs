using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmartExpenseTracker.Application.Services;
using SmartExpenseTracker.Domain.Entities;
using SmartExpenseTracker.Domain.Interfaces;

namespace SmartExpenseTracker.Tests;

public class ExpenseServiceTests
{
    private readonly Mock<IExpenseRepository> _repoMock;
    private readonly ExpenseService _service;

    public ExpenseServiceTests()
    {
        _repoMock = new Mock<IExpenseRepository>();
        _service = new ExpenseService(
            NullLogger<ExpenseService>.Instance,
            _repoMock.Object
        );
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllExpenses()
    {
        // Arrange
        var expenses = new List<Expense>
        {
            new Expense { Id = Guid.NewGuid(), Description = "Lunch", Amount = 150 },
            new Expense { Id = Guid.NewGuid(), Description = "Taxi",  Amount = 80  }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expenses);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsExpense()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expense = new Expense { Id = id, Description = "Coffee", Amount = 50 };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(expense);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Expense?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ValidExpense_ReturnsCreatedExpense()
    {
        // Arrange
        var expense = new Expense { Id = Guid.NewGuid(), Description = "Dinner", Amount = 200 };
        _repoMock.Setup(r => r.CreateAsync(expense)).ReturnsAsync(expense);

        // Act
        var result = await _service.CreateAsync(expense);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dinner", result.Description);
    }
}