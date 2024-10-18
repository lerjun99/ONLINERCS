using Microsoft.AspNetCore.Mvc;
using RCS.Models;
using System.Diagnostics;

namespace RCS.Controllers
{
    public class MiscellaneousController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public MiscellaneousController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult CashBalancing()
        {
            return View();
        }
        public IActionResult CancelOR()
        {
            return PartialView("CancelOR");
        }


    }
}
