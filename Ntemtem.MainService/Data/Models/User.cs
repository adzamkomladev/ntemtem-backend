using Convey.Types;

namespace Ntemtem.MainService.Data.Models;

public class User : IIdentifiable<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public string Email { get; set; }

    public string Username { get; set; } = null!;
}