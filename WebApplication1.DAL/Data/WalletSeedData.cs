using Microsoft.EntityFrameworkCore;
using WebApplication1.BLL.Models;

namespace WebApplication1.DAL.Data;

public static class WalletSeedData
{
    // #4: Seed realistic service providers and customer requisites for the payment workflow.
    public static async Task SeedAsync(MobileContext context, CancellationToken cancellationToken = default)
    {
        if (await context.ServiceProviders.AnyAsync(cancellationToken)) return;

        context.ServiceProviders.AddRange(
            new ServiceProvider
            {
                Name = "Mobilink",
                Category = "Мобильная связь",
                Accounts = [new ServiceAccount { Identifier = "0555123456", CustomerName = "Демо абонент", Balance = 0m }]
            },
            new ServiceProvider
            {
                Name = "FiberNet",
                Category = "Интернет",
                Accounts = [new ServiceAccount { Identifier = "12345678", CustomerName = "Демо абонент", Balance = 0m }]
            },
            new ServiceProvider
            {
                Name = "City Utilities",
                Category = "Коммунальные услуги",
                Accounts = [new ServiceAccount { Identifier = "LC-100200", CustomerName = "Демо абонент", Balance = 0m }]
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}
