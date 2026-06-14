using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WaterTreatmentSystem.Models;

namespace WaterTreatmentSystem.Data
{
    // 🔐 يرث الآن من IdentityDbContext لإضافة جداول المستخدمين والصلاحيات تلقائياً
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // جداول المحطات والمنطق الهندسي الخاص بنا
        public DbSet<Plant> Plants { get; set; }
        public DbSet<WaterQualityAnalysis> WaterQualityAnalyses { get; set; }
        public DbSet<PlantComponent> PlantComponents { get; set; }
        public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ⚠️ ضروري جداً عند استخدام الـ Identity لتجنب أخطاء الـ Mapping
            base.OnModelCreating(modelBuilder);

            // إعدادات العلاقات وحماية البيانات الهندسية من الحذف العشوائي
            modelBuilder.Entity<WaterQualityAnalysis>()
                .HasOne(q => q.Plant)
                .WithMany(p => p.QualityAnalyses)
                .HasForeignKey(q => q.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlantComponent>()
                .HasOne(c => c.Plant)
                .WithMany(p => p.Components)
                .HasForeignKey(c => c.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaintenanceLog>()
                .HasOne(m => m.Plant)
                .WithMany(p => p.MaintenanceLogs)
                .HasForeignKey(m => m.PlantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}