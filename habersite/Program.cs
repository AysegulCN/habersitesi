using Microsoft.EntityFrameworkCore;
using habersite.Models;

var builder = WebApplication.CreateBuilder(args);

// Connection string'i burada tanýmlýyoruz.
var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=HabersiteDB_Yeni;Trusted_Connection=True;TrustServerCertificate=True";

// --- KRÝTÝK DÜZELTME: DbContext Servisini Ekleme ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            // Transient error resiliency (geçici hata dayanýklýlýðý) eklendi
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));
// ----------------------------------------------------

builder.Services.AddControllersWithViews();

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