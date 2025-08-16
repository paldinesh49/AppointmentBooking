using AppointmentBooking.Infrastructure.Data;
using AppointmentBooking.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppointmentBooking.Web.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _reservedPaths =
            { "account", "admin", "api", "static", "_framework", "css", "js", "images" };

        public TenantResolutionMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            ICurrentTenant currentTenant,
            AppDbContext db,
            UserManager<ApplicationUser>? userManager = null)
        {
            // 1) If user is authenticated and has TenantId, use that
            if (context.User?.Identity?.IsAuthenticated == true && userManager != null)
            {
                var userId = userManager.GetUserId(context.User);
                if (!string.IsNullOrEmpty(userId))
                {
                    var appUser = await userManager.FindByIdAsync(userId);
                    if (appUser?.TenantId != null)
                    {
                        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == appUser.TenantId);
                        if (tenant != null)
                        {
                            currentTenant.TenantId = tenant.Id;
                            currentTenant.Slug = tenant.Slug;
                            currentTenant.BusinessName = tenant.BusinessName;
                            currentTenant.PrimaryColor = tenant.PrimaryColor;
                            currentTenant.SecondaryColor = tenant.SecondaryColor;

                            // Store branding in HttpContext for views/layout
                            context.Items["BusinessName"] = tenant.BusinessName;
                            context.Items["PrimaryColor"] = tenant.PrimaryColor;
                            context.Items["SecondaryColor"] = tenant.SecondaryColor;
                            context.Items["LogoUrl"] = tenant.LogoUrl;

                            await _next(context);
                            return;
                        }
                    }
                }
            }

            // 2) Try resolve slug from first path segment
            var path = context.Request.Path.Value ?? "";
            var trimmed = path.Trim('/');
            var parts = trimmed.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                var candidate = parts[0].ToLowerInvariant();
                if (!_reservedPaths.Contains(candidate))
                {
                    var tenant = await db.Tenants
                        .FirstOrDefaultAsync(t => t.Slug.ToLower() == candidate);
                    if (tenant != null)
                    {
                        currentTenant.TenantId = tenant.Id;
                        currentTenant.Slug = tenant.Slug;
                        currentTenant.BusinessName = tenant.BusinessName;
                        currentTenant.PrimaryColor = tenant.PrimaryColor;
                        currentTenant.SecondaryColor = tenant.SecondaryColor;

                        // Store branding in HttpContext for views/layout
                        context.Items["BusinessName"] = tenant.BusinessName;
                        context.Items["PrimaryColor"] = tenant.PrimaryColor;
                        context.Items["SecondaryColor"] = tenant.SecondaryColor;
                        context.Items["LogoUrl"] = tenant.LogoUrl;

                        await _next(context);
                        return;
                    }
                }
            }

            // 3) No tenant found => leave null
            currentTenant.TenantId = null;
            currentTenant.Slug = null;
            currentTenant.BusinessName = null;
            currentTenant.PrimaryColor = null;
            currentTenant.SecondaryColor = null;

            await _next(context);
        }
    }
}
