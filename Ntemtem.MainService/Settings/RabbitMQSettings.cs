namespace Ntemtem.MainService.Settings;

public class RabbitMQSettings
{
    public string Host { get; set; } = null!;

    public ushort Port { get; set; } = default;

    public string Username { get; set; } = null!;
    
    public string Password { get; set; } = null!;
}