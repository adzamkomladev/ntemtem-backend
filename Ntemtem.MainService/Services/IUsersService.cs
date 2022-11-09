using Ntemtem.MainService.Data.Models;

namespace Ntemtem.MainService.Services;

public interface IUsersService
{
    public Task<List<User>> GetAsync();

    public  Task<User?> GetAsync(string id);

    public Task CreateAsync(User newUser);
    
    public Task CreateWithoutEventAsync(User newUser);


    public Task UpdateAsync(string id, User updatedUser);

    public Task RemoveAsync(string id);
}