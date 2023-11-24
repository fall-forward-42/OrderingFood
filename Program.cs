using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderingFood.Data;
using OrderingFood.Interfaces;
using OrderingFood.Services;
using System.Configuration;
using System.Data;
using System.Text;

var _builder = WebApplication.CreateBuilder(args);

// Add services to the container.
_builder.Services.AddControllersWithViews();
//db context
_builder.Services.AddDbContext<FoodieContext>(options =>
{
    options.UseSqlServer(_builder.Configuration.GetConnectionString("Foodie"));
   

}


 );

_builder.Services.AddDistributedMemoryCache();

//session
_builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AdventureWorks.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.IsEssential = true;
});
//interface and services
_builder.Services.AddTransient<IBufferedFileUploadService, BufferedFileUploadLocalService>();
_builder.Services.AddTransient<IJwtAuthenService, JwtAuthenService>();
_builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//toast
_builder.Services.AddNotyf(cof =>
{
    cof.DurationInSeconds = 10;
    cof.IsDismissable = true;
    cof.Position = NotyfPosition.BottomRight;
});

//authen with cookie
_builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
    });

//run the bulider
var app = _builder.Build();

//help to injected into the controller
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();
app.UseSession();

//authen and author
app.UseAuthentication();
app.UseAuthorization();

//policy for authen cookie
var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};
app.UseCookiePolicy(cookiePolicyOptions);

app.MapAreaControllerRoute(
    name: "MyAreaAdmin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
 
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
