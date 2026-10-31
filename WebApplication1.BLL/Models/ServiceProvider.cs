namespace WebApplication1.BLL.Models;

public sealed class ServiceProvider
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<ServiceAccount> Accounts { get; set; } = [];
}
