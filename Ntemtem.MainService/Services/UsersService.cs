using Convey.Persistence.MongoDB;
using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Ntemtem.MainService.Data.Messages.Users;
using Ntemtem.MainService.Data.Models;
using Ntemtem.MainService.Settings;

namespace Ntemtem.MainService.Services;

public class UsersService : IUsersService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMongoRepository<User, Guid> _userRepository;

    public UsersService(
        IServiceScopeFactory serviceScopeFactory,
        IMongoRepository<User, Guid> userRepository
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _userRepository = userRepository;
    }

    public async Task<List<User>> GetAsync() => (List<User>) await _userRepository.FindAsync(_ => true);

    public async Task<User?> GetAsync(string id) => await _userRepository.GetAsync(new Guid(id));

    public async Task CreateAsync(User newUser)
    {
        await _userRepository.AddAsync(newUser);

        using var scope = _serviceScopeFactory.CreateScope();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        await publishEndpoint.Publish<UserCreated>(new
        {
            user = newUser
        });
    }

    public async Task UpdateAsync(string id, User updatedUser) =>
        await _userRepository.UpdateAsync(updatedUser, x => x.Id == new Guid(id));

    public async Task RemoveAsync(string id) => await _userRepository.DeleteAsync(new Guid(id));

    public async Task CreateWithoutEventAsync(User newUser) => await _userRepository.AddAsync(newUser);
}