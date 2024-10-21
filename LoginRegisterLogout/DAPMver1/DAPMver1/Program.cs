using DAPMver1.Data;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

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
