using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Infrastructure.Data;
using AppointmentBooking.Infrastructure.Tenancy;
using AppointmentBooking.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentBooking.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            AppDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ===== TENANT CREATION =====
        [HttpGet]
        public IActionResult CreateTenant()
        {
            return View(new CreateTenantViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant(CreateTenantViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var tenant = new Tenant
            {
                BusinessName = model.BusinessName,
                Slug = model.Slug
            };
            _dbContext.Tenants.Add(tenant);
            await _dbContext.SaveChangesAsync();

            var ownerUser = new ApplicationUser
            {
                UserName = model.OwnerEmail,
                Email = model.OwnerEmail,
                TenantId = tenant.Id
            };
            var result = await _userManager.CreateAsync(ownerUser, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(ownerUser, "TenantOwner");
                TempData["Success"] = "Tenant created successfully!";
                return RedirectToAction(nameof(CreateTenant));
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // ===== ROLE CREATION =====
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "Role name is required.");
                return View();
            }

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                ModelState.AddModelError("", "Role already exists.");
                return View();
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                TempData["Success"] = "Role created successfully!";
                return RedirectToAction(nameof(CreateRole));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View();
        }

        // ===== ASSIGN ROLE TO USER =====
        [HttpGet]
        public async Task<IActionResult> AssignRole()
        {
            var model = new AssignRoleViewModel
            {
                Users = _userManager.Users.Select(u => new UserItem
                {
                    Id = u.Id,
                    Email = u.Email
                }).ToList(),
                Roles = _roleManager.Roles.Select(r => r.Name).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Users = _userManager.Users.Select(u => new UserItem
                {
                    Id = u.Id,
                    Email = u.Email
                }).ToList();
                model.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync(model.RoleName))
            {
                ModelState.AddModelError("", "Role does not exist.");
                return View(model);
            }

            // Remove user from all roles first (optional)
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Assign new role
            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (result.Succeeded)
            {
                TempData["Success"] = "Role assigned successfully!";
                return RedirectToAction("AssignRole");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Dashboard()
        {
            return View();
        }
        [Authorize(Roles = "TenantOwner")]
        public IActionResult TenantOwnerDashboard()
        {
            // You can pass tenant info if needed via ViewBag
            var tenantId = HttpContext.Items["TenantId"];
            ViewBag.TenantId = tenantId;
            return View();
        }


    }
}
