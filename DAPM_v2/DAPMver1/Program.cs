using DAPMver1.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Configuration;
using static Azure.Core.HttpHeader;
using DAPMver1.Common;
using Common = DAPMver1.Common.Common;
using Microsoft.AspNetCore.SignalR;
using DAPMver1.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
//
builder.Services.AddScoped<Common>();
        //db
        builder.Services.AddDbContext<DapmTrangv1Context>(options =>
        {
            options.UseSqlServer(builder.Configuration["DAPM"]);
        });

//
builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
            options.Cookie.HttpOnly = true; // Bảo mật cookie session
            options.Cookie.IsEssential = true; // Đảm bảo cookie luôn được gửi ngay cả khi người dùng không đồng ý với cookie khác
        });
        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();
        //
        // Cấu hình routing hỗ trợ Areas
        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=AdminHome}/{action=Index}/{id?}");
//
        app.UseSession();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapHub<ChatHub>("/chatHub");
// Configure the HTTP request pipeline.

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

       
     

        app.UseAuthorization();

   

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddSignalR();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //add configuration/middleware here

    }

}
