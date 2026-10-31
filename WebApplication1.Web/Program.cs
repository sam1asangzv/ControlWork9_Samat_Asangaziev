using WebApplication1.BLL;
using WebApplication1.DAL;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace WebApplication1;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        builder.Services.AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization(options =>
                options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(WalletResources)));
        builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");
        builder.Services.AddBusinessLogic();
        builder.Services.AddDataAccess(builder.Configuration);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("ru-RU"),
            SupportedCultures = [new CultureInfo("ru-RU")],
            SupportedUICultures = [new CultureInfo("ru-RU")]
        });
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        await app.Services.InitializeWalletDatabaseAsync();

        app.Run();
    }
}
