using MassTransit;
using Ntemtem.MainService.Data.Messages.Users;
using Ntemtem.MainService.Data.Models;
using Ntemtem.MainService.Services;

namespace Ntemtem.MainService.Consumers.Users;

public class UserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly ILogger<UserCreatedConsumer> _logger;
    private readonly IUsersService _usersService;

    public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger, IUsersService usersService)
    {
        _logger = logger;
        _usersService = usersService;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var user = context.Message.User;

        for (var i = 0; i < 10; i++)
            await _usersService.CreateWithoutEventAsync(new User
            {
                Email = i + user.Email,
                Username = i + user.Username,
                Name = i + user.Name
            });

        _logger.LogInformation("THESE USERS WERE CREATED BY {Email} with ID: {Id}", user.Email, user.Id);

    }
}