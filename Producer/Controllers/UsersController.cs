using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
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

        await _producer.ProduceAsync("users", new Message<Null, string>
        {
            Value = userJson
        });

        _logger.LogInformation("User message produced successfully");
        return Ok(user);
    }
}
