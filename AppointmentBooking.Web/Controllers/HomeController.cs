using System.Diagnostics;
using AppointmentBooking.Infrastructure.Tenancy;
using AppointmentBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentBooking.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICurrentTenant _currentTenant;

        public HomeController(ILogger<HomeController> logger, ICurrentTenant currentTenant)
        {
            _logger = logger;
            _currentTenant = currentTenant;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(_currentTenant.Slug))
            {
                return View("Marketing");
            }
            else
            {
                return RedirectToAction(nameof(TenantHome));
            }
        }

        public IActionResult TenantHome()
        {
            ViewBag.BusinessName = HttpContext.Items["BusinessName"];
            ViewBag.LogoUrl = HttpContext.Items["LogoUrl"];
            ViewBag.PrimaryColor = HttpContext.Items["PrimaryColor"];
            ViewBag.SecondaryColor = HttpContext.Items["SecondaryColor"];

            return View();
        }
       
        public IActionResult Marketing()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
