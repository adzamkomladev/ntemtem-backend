using Ntemtem.MainService.Data.Models;

namespace Ntemtem.MainService.Data.Messages.Users;

public interface UserCreated
{
    public User User { get; set; }
}