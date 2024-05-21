using DinkToPdf;
using DinkToPdf.Contracts;
using MarketWebApp.Data;
using MarketWebApp.Models;
using MarketWebApp.Models.Entity;
using MarketWebApp.Repository.LocationRepository;
using MarketWebApp.Repository.ProductRepository;
using MarketWebApp.Repository.SupplierRepository;
using MarketWebApp.Reprository;
using MarketWebApp.Reprository.CategoryReprositry;
using MarketWebApp.Reprository.OrderReprository;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace MarketWebApp
{
    public class Program

    {
        public static void Main(string[] args)
        {
            #region Comments
            //        var builder = WebApplication.CreateBuilder(args);

            //        // Add services to the container.
            //        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            //        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //            options.UseSqlServer(connectionString));

            //        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
            //            .AddEntityFrameworkStores<ApplicationDbContext>();

            //        builder.Services.AddControllersWithViews();


            //        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            //        builder.Services.AddSession();
            //        builder.Services.AddRazorPages();

            //        builder.Services.ConfigureApplicationCookie(options =>
            //        {

            //            options.LoginPath = "/Identity/Account/Login";
            //            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            //        });


            //        var app = builder.Build();

            //        // Configure the HTTP request pipeline.
            //        if (app.Environment.IsDevelopment())
            //        {
            //            app.UseMigrationsEndPoint();
            //        }
            //        else
            //        {
            //            app.UseExceptionHandler("/Home/Error");
            //            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //            app.UseHsts();
            //        }

            //        app.UseHttpsRedirection();
            //        app.UseStaticFiles();

            //        app.UseRouting();

            //        app.UseAuthorization();

            //        app.UseSession();

            //        app.MapControllerRoute(
            //            name: "default",
            //            pattern: "{controller=Home}/{action=Index}/{id?}");
            //        app.MapRazorPages();

            //        app.Run();
            //    }
            //}
            #endregion

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));

            builder.Services.AddMvc().AddSessionStateTempDataProvider();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderAdminRepository, OrderAdminRepository>();
            builder.Services.AddScoped<IOrderProductRepository, OrderProductRepository>();
            builder.Services.AddScoped<EmailService>();

            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            builder.Services.AddSession();
            builder.Services.AddRazorPages();

            builder.Services.ConfigureApplicationCookie(options =>
            {

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }



   
           

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

           

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                  name: "paypal",
                  pattern: "PaypalController/{action}",
                  defaults: new { controller = "Paypal" });
            app.MapRazorPages();

            app.Run();
        }
    }
}
