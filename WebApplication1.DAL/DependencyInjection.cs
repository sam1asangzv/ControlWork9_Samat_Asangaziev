using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.BLL.Interfaces;
using WebApplication1.BLL.Models;
using WebApplication1.DAL.Data;
using WebApplication1.DAL.Repositories;

namespace WebApplication1.DAL;

public static class DependencyInjection
{
    public static async Task InitializeWalletDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<MobileContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);
        await WalletSeedData.SeedAsync(context, cancellationToken);
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException();

        services.AddDbContext<MobileContext>(options => options.UseNpgsql(connectionString));
        services.AddIdentity<WalletUser, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<MobileContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IWalletRepository, WalletRepository>();
        return services;
    }
}
