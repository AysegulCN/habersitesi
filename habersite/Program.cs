using Microsoft.EntityFrameworkCore;
using habersite.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Connection string - kesin çalýþan
var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=HabersiteDB_Yeni;Trusted_Connection=True;TrustServerCertificate=True"; builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();