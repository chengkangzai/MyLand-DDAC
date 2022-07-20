using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLand.Data;
using Microsoft.AspNetCore.Identity;
using MyLand.Areas.Identity.Data;


namespace MyLand.Controllers
{
    [Authorize]
    public class AppController : Controller
    {
        private readonly MyLandContext _context;
        private readonly UserManager<MyLandUser> _userManager;

        public AppController(MyLandContext context, UserManager<MyLandUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        [HttpGet]
        public IActionResult UNAUTHORIZED()
        {
            return View("Unauthorized");
        }

        [HttpGet]
        public IActionResult NOTFOUND()
        {
            return View("NotFound");
        }
    }
}
