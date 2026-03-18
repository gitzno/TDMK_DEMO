using Microsoft.EntityFrameworkCore;
using TDMK_API_DEMO.Models;

namespace TDMK_API_DEMO.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Telemetry> Telemetries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tối ưu: Đánh Index cho cột CreatedAt vì ta thường xuyên sắp xếp theo thời gian để vẽ biểu đồ
            modelBuilder.Entity<Telemetry>()
                .HasIndex(t => t.CreatedAt);

            // Giới hạn độ dài URL ảnh để tối ưu dung lượng DB
            modelBuilder.Entity<Telemetry>()
                .Property(t => t.ImageUrl)
                .HasMaxLength(500);
        }
    }
}
