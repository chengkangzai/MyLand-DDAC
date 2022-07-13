using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLand.Data;
using MyLand.Models;
using Microsoft.AspNetCore.Identity;
using MyLand.Areas.Identity.Data;
using MyLand.Services;


namespace MyLand.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        private readonly MyLandContext _context;
        private readonly UserManager<MyLandUser> _userManager;

        public PropertiesController(MyLandContext context, UserManager<MyLandUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Properties
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role == MyLandUser.ROLE_USER)
            {
                return Unauthorized();
            }
            var properties = await _context.Property
                .Include(property => property.User)
                .Where(property => property.IsActive)
                .ToListAsync();
            return View(properties);
        }

        // GET: Moderate Properties
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);
            //TODO check the user role
            var properties = await _context.Property
                .Where(p => p.User.Id == user.Id)
                .ToListAsync();
            return View(properties);
        }

        // GET: Moderate Properties
        [HttpGet]
        public async Task<IActionResult> Moderate()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_ADMIN)
            {
                return Unauthorized();
            }
            var properties = await _context.Property
                .Include(m => m.User)
                .ToListAsync();

            return View(properties);
        }

        // GET: Properties/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var property = await _context.Property
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store([Bind("Id,Type,Title,Description,Price,Size,Date")] Property property)
        {
            var user = await _userManager.GetUserAsync(User);
            property.User = user;
            property.IsActive = true;
            property.Date = DateTime.Now;

            //get the file from the form
            var formCollection = Request.Form;
            var images = formCollection.Files;

            foreach (var image in images)
            {
                await S3Service.UploadImages(image.FileName, image.OpenReadStream());
            }
            property.Photo = images.First().FileName;

            if (!ModelState.IsValid)
            {
                return View("Create", property);
            }
            _context.Add(property);
            await _context.SaveChangesAsync();
            return RedirectToAction(user.Role == MyLandUser.ROLE_USER ? nameof(Manage) : nameof(Index));
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var property = await _context.Property.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (!(property.User == user || user.Role == 1))
            {
                return Unauthorized();
            }

            return View(property);
        }

        // POST: Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,
            [Bind("Id,Type,Title,Description,Price,Size,Photo,Date")] Property property)
        {
            if (id == null)
            {
                return NotFound();
            }
            var target = await _context.Property.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (!(target.User == user || user.Role == 1))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View("Edit", property);
            }

            property.User.UserName = target.User.UserName;
            property.IsActive = true;
            try
            {
                _context.Update(property);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropertyExists(property.Id))
                {
                    return NotFound();
                }
                return NotFound($"An error has occured");
            }
            return RedirectToAction(nameof(Manage));
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != 1)
            {
                return Unauthorized();
            }
            var property = await _context.Property.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        // POST: Properties/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var property = await _context.Property.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            if (!(user.Role == 1 || user == property.User))
            {
                return Unauthorized();
            }
            property.IsActive = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }
        
        // POST: Properties/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var property = await _context.Property.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            if (!(user.Role == 1 || user == property.User))
            {
                return Unauthorized();
            }
            property.IsActive = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }

        // POST: Properties/Notify/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Notify(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var property = await _context.Property.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            //TODO SNS
            //target.User.UserEmail
            //owner.UserEmail
            throw new NotImplementedException($"This function will send email");
        }

        private bool PropertyExists(int id)
        {
            return _context.Property.Any(e => e.Id == id);
        }
    }
}
