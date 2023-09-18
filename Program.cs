
using Inventarisation.Interfaces;
using Inventarisation.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using NPOI.SS.Formula.Functions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = "YourCookieName"; // Имя куки
            options.LoginPath = "/User/Login";    // URL для перенаправления на страницу входа
            options.LogoutPath = "/Account/Logout";  // URL для перенаправления на страницу выхода
            options.AccessDeniedPath = "/User/AccessDenied";
        });
builder.Services.AddAuthorization();
builder.Services.AddScoped<IBDWork,BDWork>();

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
app.MapHub<ScanHub>("/scanHub"); // Указываем URL для хаба ScanHub
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}");

app.Run();
