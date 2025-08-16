using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Application.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppointmentBooking.Web.Controllers
{
    
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointments;
        private readonly IServiceService _services;
        private readonly ICustomerService _customers;

        public AppointmentsController(
            IAppointmentService appointments,
            IServiceService services,
            ICustomerService customers)
        {
            _appointments = appointments;
            _services = services;
            _customers = customers;
        }
        [Authorize(Roles = "SuperAdmin,TenantOwner,Staff")]
        public async Task<IActionResult> Index()
        {
            var items = await _appointments.GetAllAsync();
            return View(items);
        }
        [Authorize(Roles = "SuperAdmin,TenantOwner,Staff,Customer")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new CreateAppointmentRequest { StartTime = DateTime.Now.AddHours(1) });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentRequest req)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(req);
            }

            var (ok, error) = await _appointments.CreateAsync(req);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to create.");
                await PopulateDropdowns();
                return View(req);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,TenantOwner,Staff")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var (ok, error) = await _appointments.UpdateStatusAsync(id, status);
            if (!ok) TempData["Error"] = error;
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns()
        {
            var services = await _services.GetAllAsync();
            var customers = await _customers.GetAllAsync();
            ViewBag.Services = new SelectList(services, "Id", "Name");
            ViewBag.Customers = new SelectList(customers, "Id", "FullName");
        }
    }
}
