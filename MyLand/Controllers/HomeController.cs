using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyLand.Data;
using MyLand.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyLand.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyLandContext _context;

        public HomeController(ILogger<HomeController> logger, MyLandContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var properties = _context.Property
                .Include(m => m.User)
                .Where(m => m.IsActive == true)
                .Take(3)
                .ToList();
            return View(properties);
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
