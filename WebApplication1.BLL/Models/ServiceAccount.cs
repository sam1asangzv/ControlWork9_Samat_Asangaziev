namespace WebApplication1.BLL.Models;

public sealed class ServiceAccount
{
    public int Id { get; set; }
    public int ServiceProviderId { get; set; }
    public ServiceProvider Provider { get; set; } = null!;
    public string Identifier { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
