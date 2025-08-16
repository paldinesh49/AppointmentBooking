using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Infrastructure.Data; // Ensure this points to your ApplicationUser class
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppointmentBooking.Web.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await db.Database.MigrateAsync();

            // 1️⃣ Seed roles
            var roles = new[] { "SuperAdmin", "TenantOwner", "Staff", "Customer" };
            foreach (var r in roles)
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole(r));

            // 2️⃣ Seed Super Admin
            var super = await userMgr.FindByEmailAsync("super@saas.local");
            if (super == null)
            {
                super = new ApplicationUser
                {
                    UserName = "super@saas.local",
                    Email = "super@saas.local",
                    EmailConfirmed = false,
                };
                await userMgr.CreateAsync(super, "Admin#12345");
                await userMgr.AddToRoleAsync(super, "SuperAdmin");
            }

            // 3️⃣ Ensure "Default Tenant" exists
            var defaultTenant = await db.Tenants.FirstOrDefaultAsync(t => t.Slug == "default");
            if (defaultTenant == null)
            {
                defaultTenant = new Tenant
                {
                    Name = "Default Tenant",
                    Slug = "default"
                };
                db.Tenants.Add(defaultTenant);
                await db.SaveChangesAsync();
            }

            // 4️⃣ Assign orphaned appointments to default tenant
            var orphanAppointments = await db.Appointments
                .Where(a => a.TenantId == 0 || a.TenantId == null)
                .ToListAsync();
            if (orphanAppointments.Any())
            {
                foreach (var appt in orphanAppointments)
                    appt.TenantId = defaultTenant.Id;
                await db.SaveChangesAsync();
            }

            // 5️⃣ Seed a sample tenant & owner
            if (!await db.Tenants.AnyAsync(t => t.Slug == "barber-one"))
            {
                var tenant = new Tenant { Name = "Barber One", Slug = "barber-one" };
                db.Tenants.Add(tenant);
                await db.SaveChangesAsync();

                var owner = new ApplicationUser
                {
                    UserName = "owner@barber.local",
                    Email = "owner@barber.local",
                    EmailConfirmed = true,
                    TenantId = tenant.Id
                };
                await userMgr.CreateAsync(owner, "Owner#12345");
                await userMgr.AddToRoleAsync(owner, "TenantOwner");

                db.Services.AddRange(
                    new Service { Name = "Haircut", Price = 15m, DurationMinutes = 30, TenantId = tenant.Id },
                    new Service { Name = "Shave", Price = 10m, DurationMinutes = 20, TenantId = tenant.Id },
                    new Service { Name = "Haircut + Shave", Price = 22m, DurationMinutes = 50, TenantId = tenant.Id }
                );

                db.Customers.AddRange(
                    new Customer { FullName = "John Doe", Email = "john@example.com", PhoneNumber = "9999999999", TenantId = tenant.Id },
                    new Customer { FullName = "Aisha Khan", Email = "aisha@example.com", PhoneNumber = "8888888888", TenantId = tenant.Id }
                );

                await db.SaveChangesAsync();
            }
        }
    }

}
