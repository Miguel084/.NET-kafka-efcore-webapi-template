using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Data.Config;
using Shared.Domain.Data.Dto;
using Shared.Domain.Data.Models;

namespace Producer.Controllers;

public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IProducer<Null, string> _producer;
    private readonly EfDbContext _dbContext;

    public UsersController(ILogger<UsersController> logger, IProducer<Null, string> userService, EfDbContext dbContext)
    {
        _logger = logger;
        _producer = userService;
        _dbContext = dbContext;
    }

    [HttpPost("users")]
    public async Task<IActionResult> Create([FromForm] CreateUserFullDto user)
    {
        var validationMessage = user.ValidateUser();

        if (!string.IsNullOrEmpty(validationMessage))
        {
            _logger.LogWarning("User validation failed: {ValidationMessage}", validationMessage);
            return BadRequest(validationMessage);
        }

        _logger.LogInformation("Creating user: {Name}", user.Name);

        var userMap = new Users
        {
            Name = user.Name,
            Email = user.Email,
            Address = user.Address != null ? new Address
            {
                Street = user.Address.Street,
                City = user.Address.City,
                State = user.Address.State,
                ZipCode = user.Address.ZipCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            } : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.Users.Add(userMap);
        await _dbContext.SaveChangesAsync();

        var userJson = JsonSerializer.Serialize(user);

        _logger.LogInformation("Producing user message: {UserJson}", userJson);

        var partition = Math.Abs((user.Email ?? "").GetHashCode()) % 3;
        await _producer.ProduceAsync(
            new TopicPartition("users", partition),
            new Message<Null, string> { Value = userJson }
        );

        _logger.LogInformation("User message produced successfully");
        return Ok(user);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _dbContext.Users.Include(w => w.Wallet)
            .ThenInclude(w => w.Transactions)
            .AsNoTracking()
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.Email,
                WalletBalance = u.GetWalletBalance(),
                Transactions = u.Wallet!.Transactions.Select(t => new
                {
                    t.WalletTransactionId,
                    t.Amount,
                    TransactionType = t.TransactionType.ToString(),
                    t.TransactionDate,
                    t.Description
                }).ToList(),
            })
    .ToListAsync();

        if (users == null || !users.Any())
        {
            _logger.LogInformation("No users found");
            return NotFound("No users found");
        }


        return Ok(users);
    }

    [HttpPost("users/CreditCash")]
    public async Task<IActionResult> CreditCash([FromForm] int userId, [FromForm] decimal amount)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found", userId);
            return NotFound($"User with ID {userId} not found");
        }

        if (amount <= 0)
        {
            _logger.LogWarning("Invalid amount: {Amount}", amount);
            return BadRequest("Amount must be greater than zero");
        }

        var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.User.Id == userId);
        if (wallet == null)
        {
            wallet = new Wallet { User = user };
            _dbContext.Wallets.Add(wallet);
        }

        var transaction = new WalletTransaction
        {
            Wallet = wallet,
            Amount = amount,
            TransactionType = WalletTransaction.TransactionTypeEnum.Deposit,
            TransactionDate = DateTime.UtcNow,
            Description = "Credit cash",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        transaction.ValidateTransaction();

        _dbContext.WalletTransactions.Add(transaction);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Credited {Amount} to user {UserId}'s wallet", amount, userId);

        var message = new
        {
            UserId = userId,
            Amount = amount,
            TransactionType = "Credit",
            transaction.TransactionDate,
            transaction.Description
        };

        await _producer.ProduceAsync(
            new TopicPartition("usersCredits", new Partition(Math.Abs(userId.GetHashCode()) % 3)),
            new Message<Null, string> { Value = message.ToString() }
        );

        return Ok(new { Message = $"Credited {amount} to user {userId}'s wallet" });


    }

}
