using Microsoft.AspNetCore.Mvc;
using RCS.Models;
using System.Diagnostics;

namespace RCS.Controllers
{
    public class InquiryController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public InquiryController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

    
        
    }
}
