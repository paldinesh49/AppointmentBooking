using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Infrastructure.Data;
using AppointmentBooking.Infrastructure.Services;
using AppointmentBooking.Infrastructure.Tenancy;
using AppointmentBooking.Web.Data;
using AppointmentBooking.Web.Middleware;
using Microsoft.AspNetCore.Identity;
// ... rest of your code remains unchanged
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // ADD THIS LINE


namespace AppointmentBooking.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<ICurrentTenant, CurrentTenant>();
            builder.Services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                options.SignIn.RequireConfirmedEmail = false; // Enable email confirmation
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            // register current tenant
            builder.Services.AddScoped<ICurrentTenant, CurrentTenant>();
            // DI wiring
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
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
            app.UseMiddleware<TenantResolutionMiddleware>(); // <-- tenant set here
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            await SeedData.SeedAsync(app);
            app.Run();
        }
    }
}
