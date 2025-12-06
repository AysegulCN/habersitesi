// Models/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;

namespace habersite.Models
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor, Program.cs'den bağlantı ayarlarını alır.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Entity Framework Core için veritabanındaki tabloları temsil eden DbSet'ler
        // Kategoriler tablosu
        public DbSet<Category> Categories { get; set; }

        // Haberler tablosu
        public DbSet<News> News { get; set; }

        // Not: Eğer tablolarınızın ismi farklıysa (örneğin VeritabaniNews),
        // OnModelCreating metodunda tablo ismini override edebilirsiniz, 
        // ancak şimdilik bu yapı yeterli.

        // Aşağıdaki kod, eğer modellerinizde tanımlamadıysanız, 
        // string alanların null olabileceği uyarısını (CS8618) görmezden gelmek için eklenebilir.
        // Ama en iyisi modellerde '?' kullanmaktır.
        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tüm string alanların nullable olarak kabul edilmesini sağlar.
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entity.GetProperties().Where(p => p.ClrType == typeof(string));
                foreach (var property in properties)
                {
                    property.IsNullable = true;
                }
            }
        }
        */
    }
}