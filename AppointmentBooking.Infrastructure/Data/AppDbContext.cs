using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppointmentBooking.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext
    {
        private readonly ICurrentTenant _currentTenant;

        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenant currentTenant)
            : base(options)
        {
            _currentTenant = currentTenant;
        }
        public DbSet<ApplicationUser> applicationUsers => Set<ApplicationUser>();
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Appointment → Customer: Restrict delete
            b.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment → Service: Restrict delete
            b.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // precision for price
            b.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            b.Entity<Service>()
                .Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            b.Entity<Customer>()
                .Property(c => c.FullName)
                .HasMaxLength(120)
                .IsRequired();

            // tenant foreign keys
            b.Entity<Service>()
                .HasOne(s => s.Tenant)
                .WithMany(t => t.Services)
                .HasForeignKey(s => s.TenantId);

            b.Entity<Customer>()
                .HasOne(c => c.Tenant)
                .WithMany()
                .HasForeignKey(c => c.TenantId);

            b.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId);

            b.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId);

            // ---- Global query filters to automatically scope data to CurrentTenant ----
            b.Entity<Service>()
                .HasQueryFilter(s => !_currentTenant.TenantId.HasValue || s.TenantId == _currentTenant.TenantId);

            b.Entity<Customer>()
                .HasQueryFilter(c => !_currentTenant.TenantId.HasValue || c.TenantId == _currentTenant.TenantId);

            b.Entity<Appointment>()
                .HasQueryFilter(a => !_currentTenant.TenantId.HasValue || a.TenantId == _currentTenant.TenantId);
            // Tenant entity itself is not filtered (SuperAdmin can list tenants)
        }
    }
}
