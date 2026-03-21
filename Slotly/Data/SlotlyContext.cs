using Microsoft.EntityFrameworkCore;
using Slotly.Entities;

namespace Slotly.Data
{
    public class SlotlyContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<BusinessService> BusinessServices { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StaffService> StaffServices { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<WorkingHours> WorkingHours { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public SlotlyContext(DbContextOptions<SlotlyContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Staff>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Business)
                .WithMany()
                .HasForeignKey(s => s.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Business>()
                .HasOne(b => b.Owner)
                .WithMany()
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Business>()
                .HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusinessService>()
                .HasOne(bs => bs.Business)
                .WithMany()
                .HasForeignKey(bs => bs.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BusinessService>()
                .HasOne(bs => bs.Service)
                .WithMany()
                .HasForeignKey(bs => bs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StaffService>()
                .HasOne(ss => ss.Staff)
                .WithMany()
                .HasForeignKey(ss => ss.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StaffService>()
                .HasOne(ss => ss.BusinessService)
                .WithMany()
                .HasForeignKey(ss => ss.BusinessServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkingHours>()
                .HasOne(wh => wh.Staff)
                .WithMany()
                .HasForeignKey(wh => wh.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Business)
                .WithMany()
                .HasForeignKey(a => a.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Client)
                .WithMany()
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.StaffService)
                .WithMany()
                .HasForeignKey(a => a.StaffServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .Property(a => a.FinalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BusinessService>()
                .Property(bs => bs.BasePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StaffService>()
                .Property(ss => ss.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            modelBuilder.Entity<BusinessService>()
                .HasIndex(bs => new { bs.BusinessId, bs.ServiceId })
                .IsUnique();

            modelBuilder.Entity<StaffService>()
                .HasIndex(ss => new { ss.StaffId, ss.BusinessServiceId })
                .IsUnique();

            modelBuilder.Entity<WorkingHours>()
                .HasIndex(wh => new { wh.StaffId, wh.DayOfWeek });
        }
    }
}
