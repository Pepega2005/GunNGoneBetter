using GunNGoneBetter_DataMigrations;
using GunNGoneBetter_Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using GunNGoneBetter_DataMigrations.Data;

using GunNGoneBetter_DataMigrations.Repository;
using GunNGoneBetter_DataMigrations.Repository.IRepository;
using GunNGoneBetter_Utility.BrainTree;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddHttpContextAccessor(); // ����� ��� ������ � �������� �� View

        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = "ResidentEvil4";
            //options.IdleTimeout = TimeSpan.FromSeconds(10);
        }); // ��� ������ � ��������


        builder.Services.AddDbContext<ApplicationDbContext>(
            options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

        // ��� �������������� ��������� ������ � ��
        //builder.Services.AddDefaultIdentity<IdentityUser>().
        //    AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddIdentity<IdentityUser, IdentityRole>().
            AddDefaultUI().AddDefaultTokenProviders().
            AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddAuthentication().AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        });
        builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
        {
            facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
            facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        });
        builder.Services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
        {
            microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
            microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
        });

        builder.Services.AddTransient<IEmailSender, EmailSender>(); // EMAIL SENDER
        builder.Services.AddSingleton<IBrainTreeBridge, BrainTreeBridge>();

        builder.Services.Configure<SettingsBrainTree>(builder.Configuration.GetSection("BrainTree"));

        builder.Services.AddScoped<IRepositoryCategory, RepositoryCategory>();
        builder.Services.AddScoped<IRepositoryMyModel, RepositoryMyModel>();
        builder.Services.AddScoped<IRepositoryProduct, RepositoryProduct>();
        builder.Services.AddScoped<IRepositoryQueryHeader, RepositoryQueryHeader>();
        builder.Services.AddScoped<IRepositoryQueryDetail, RepositoryQueryDetail>();
        builder.Services.AddScoped<IRepositoryApplicationUser, RepositoryApplicationUser>();
        builder.Services.AddScoped<IRepositoryOrderHeader, RepositoryOrderHeader>();
        builder.Services.AddScoped<IRepositoryOrderDetail, RepositoryOrderDetail>();

        builder.Services.AddControllersWithViews(); // MVC

        var app = builder.Build();

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

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages(); // для определения маршрута к странице Razor

        app.UseSession(); // ���������� middleware ��� ������ � ��������

        /*app.Use((context, next) =>
        {
            context.Items["name"] = "Nemesis";
            return next.Invoke();
        });*/

        /*app.Run(x =>
        {
            //return x.Response.WriteAsync("Hello " + x.Items["name"]);
            if (x.Session.Keys.Contains("name"))
            {
                return x.Response.WriteAsync(x.Session.GetString("name"));
            }
            else
            {
                x.Session.SetString("name", "Nemesis");
                return x.Response.WriteAsync("NO");
            }
        });*/

        app.Run();
    }
}