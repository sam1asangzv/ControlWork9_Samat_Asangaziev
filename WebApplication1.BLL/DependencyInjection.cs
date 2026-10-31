using Microsoft.Extensions.DependencyInjection;
using WebApplication1.BLL.Interfaces;
using WebApplication1.BLL.Services;

namespace WebApplication1.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IWalletService, WalletService>();
        return services;
    }
}
