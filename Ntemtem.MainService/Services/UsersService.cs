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
    private readonly IMongoCollection<User> _usersCollection;

    public UsersService(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<MainServiceDatabaseSettings> mainServiceDatabaseSettings)
    {
        _serviceScopeFactory = serviceScopeFactory;
        var mongoClient = new MongoClient(
            mainServiceDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            mainServiceDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>(
            mainServiceDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<User>> GetAsync()
    {
        return await _usersCollection.Find(_ => true).ToListAsync();
    }

    public async Task<User?> GetAsync(string id)
    {
        return await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(User newUser)
    {
        await _usersCollection.InsertOneAsync(newUser);

        using var scope = _serviceScopeFactory.CreateScope();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        
        await publishEndpoint.Publish<UserCreated>(new
        {
            user = newUser
        });
    }

    public async Task UpdateAsync(string id, User updatedUser)
    {
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);
    }

    public async Task RemoveAsync(string id)
    {
        await _usersCollection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task CreateWithoutEventAsync(User newUser)
    {
        await _usersCollection.InsertOneAsync(newUser);
    }
}