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


namespace MyLand.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly MyLandContext _context;
        private readonly UserManager<MyLandUser> _userManager;
        private readonly SignInManager<MyLandUser> _signInManager;
        public PropertiesController(MyLandContext context, 
            UserManager<MyLandUser> userManager, 
            SignInManager<MyLandUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            var properties = await _context.Property.ToListAsync();
            List<MyLand.Models.Property> filteredList = new List<MyLand.Models.Property>();
            foreach (var item in properties)
            {
                item.User = await _userManager.FindByIdAsync(item.UserId);
                if (item.User == null) { item.UserId = "Not Found"; }
                else { item.UserId = item.User.FirstName + ' ' + item.User.LastName; }

                //TODO Change name to S3 image link
                item.Photo = "~/imgs/"+item.Photo;
                    
                if (item.IsActive == 1)
                { filteredList.Add(item); }
            }
            return View(properties);
        }

        // GET: Moderate Properties
        public async Task<IActionResult> Manage()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                var properties = await _context.Property.ToListAsync();
                List<MyLand.Models.Property> filteredList = new List<MyLand.Models.Property>();
                foreach (var item in properties)
                {
                    if (item.UserId == user.Id)
                    { filteredList.Add(item); }
                }
                return View(filteredList);
            }
            return NotFound($"User login required");
        }

        // GET: Moderate Properties
        public async Task<IActionResult> Moderate()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (user.Role == 1)
                {
                    var properties = await _context.Property.ToListAsync();
                    foreach (var item in properties)
                    {
                        item.User = await _userManager.FindByIdAsync(item.UserId);
                        if (item.User == null) { item.UserId = "Not Found"; }
                        else { item.UserId = item.User.FirstName + ' ' + item.User.LastName; }
                    }
                    return View(properties);
                }
                return NotFound();
            }
            return NotFound($"User login required");
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }
            var property = await _context.Property
                .FirstOrDefaultAsync(m => m.Id == id);
            if (property == null) { return NotFound(); }

            property.User = await _userManager.FindByIdAsync(property.UserId);
            if (property.User == null) { property.UserId = "Not Found"; }
            else { property.UserId = property.User.FirstName + ' ' + property.User.LastName; }

            //TODO Change name to S3 image link
            property.Photo = "~/imgs/" + property.Photo;

            return View(property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmation([Bind("Id,Type,Title,Description,Price,Size,Photo,Date")] Models.Property property)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                property.UserId = user.Id;
                property.IsActive = 1;
                property.Date = DateTime.Now;

                if (ModelState.IsValid)
                {
                    _context.Add(property);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(property);
            }
            return NotFound($"User login required");
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (id == null) { return NotFound(); }
                var property = await _context.Property.FindAsync(id);
                if (property == null) { return NotFound(); }
                if (property.UserId == user.Id | user.Role == 1) { return View(property); } 
                else { return NotFound($"Only property owner may edit"); }
            }
            return NotFound($"User login required");
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmation(int? id, [Bind("Id,Type,Title,Description,Price,Size,Photo,Date")] Models.Property property)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (id == null) { return NotFound(); }
                var target = await _context.Property.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
                if (target == null) { return NotFound(); }
                if (target.UserId == user.Id | user.Role == 1) {
                    if (ModelState.IsValid) {
                        property.User.UserName = target.User.UserName;
                        property.IsActive = 1;
                        try {
                            _context.Update(property);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException) {
                            if (!PropertyExists(property.Id)) { return NotFound(); }
                            else { return NotFound($"An error has occured"); }
                        }
                        return RedirectToAction(nameof(Manage));
                    }
                    return View(property);
                }
                else
                {
                    return NotFound($"Only property owner may edit");
                }
            }
            return NotFound($"User login required");
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (id == null) { return NotFound(); }
                if (user.Role == 1)
                {
                    var property = await _context.Property.FindAsync(id);
                    if (property == null) { return NotFound(); }

                    property.User = await _userManager.FindByIdAsync(property.UserId);
                    if (property.User == null) { property.UserId = "Not Found"; }
                    else { property.UserId = property.User.FirstName + ' ' + property.User.LastName; }


                    return View(property); 
                }
                else
                { return NotFound($"Only admin may delete"); }
            }
            return NotFound($"User login required");
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (id == null) { return NotFound(); }
                if ( user.Role == 1)
                {
                    var target = await _context.Property.FindAsync(id);
                    if (target == null) { return NotFound(); }
                    _context.Property.Remove(target);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Moderate));
                }
                else
                { return NotFound($"Only admin may delete"); }
            }
            return NotFound($"User login required");
        }

        // POST: Properties/Inactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (id == null) { return NotFound(); }
                var property = await _context.Property.FindAsync(id);
                if (property == null) { return NotFound(); }
                if (user.Role == 1 | user.Id == property.UserId)
                {
                    property.IsActive = 0;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Manage));
                }
                else
                { return NotFound($"Only owner and admin may deactivate"); }
            }
            return NotFound($"User login required");
        }

        // POST: Properties/Notify/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Notify(int? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }

                var target = await _context.Property.FindAsync(id);
                if (target == null) { return NotFound(); }
                target.User = await _userManager.FindByIdAsync(target.UserId);
                if (target.User == null) { return NotFound($"Properties poster not found"); }
                //TODO SNS
                //target.User.UserEmail
                //owner.UserEmail
                return NotFound($"This function will send email");
            }
            return NotFound($"User login required");
        }

        private bool PropertyExists(int id)
        {
            return _context.Property.Any(e => e.Id == id);
        }
    }
}
