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
            if (user.Role == MyLandUser.ROLE_CUSTOMER)
            {
                return RedirectToAction("UNAUTHORIZED", "App");
            }
            var properties = await _context.Property
                .Include(property => property.User)
                .Where(property => property.IsActive)
                .ToListAsync();
            return View(properties);
        }

        // GET: Moderate Properties
        [HttpGet]
        public async Task<IActionResult> AgentIndex()
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
        public async Task<IActionResult> ModerateIndex()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_MODERATOR)
            {
                return RedirectToAction("UNAUTHORIZED", "App");
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
                return RedirectToAction("NOTFOUND", "App");
            }
            var property = await _context.Property
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (property == null)
            {
                return RedirectToAction("NOTFOUND", "App");
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

            var images = Request.Form.Files;

            foreach (var image in images)
            {
                await S3Service.UploadImages(image.FileName, image.OpenReadStream());
            }
            property.Photo = images.First().FileName;

            if (!ModelState.IsValid)
            {
                return View("Create", property);
            }
            var count = _context.Property.Count() + 1;
            var snsService = new SNSService();
            var arn = await snsService.CreateTopic("PropertyNotificationFor" + count);
            await snsService.AddSubscriptionAsync(property.User.Email, arn);
            property.topicArn = arn;
            _context.Add(property);
            await _context.SaveChangesAsync();
            return RedirectToAction(GetIndexNameByUser());
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NOTFOUND", "App");
            }
            var property = await _context.Property.FindAsync(id);
            if (property == null)
            {
                return RedirectToAction("NOTFOUND", "App");
            }
            var user = await _userManager.GetUserAsync(User);
            if (!(property.User == user || user.Role == 1))
            {
                return RedirectToAction("UNAUTHORIZED", "App");
            }

            return View(property);
        }

        // POST: Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,
            [Bind("Id,Type,Title,Description,Price,Size,Photo,Date,IsActive")] Property property)
        {
            if (id == null)
            {
                return RedirectToAction("NOTFOUND", "App");
            }
            var target = await _context.Property
                .Include(m => m.User)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (target == null)
            {
                return RedirectToAction("NOTFOUND", "App");
            }
            var user = await _userManager.GetUserAsync(User);
            if (!(target.User == user || user.Role == MyLandUser.ROLE_MODERATOR))
            {
                return RedirectToAction("UNAUTHORIZED", "App");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View("Edit", property);
            }
            var images = Request.Form.Files;
            if (images.Count > 0)
            {
                var image = images.First();
                await S3Service.UploadImages(image.FileName, image.OpenReadStream());
                await S3Service.DeleteImage(property.Photo);
                target.Photo = image.FileName;
            }
            _context.Update(target);
            await _context.SaveChangesAsync();
            return RedirectToAction(GetIndexNameByUser());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NOTFOUND", "App");
            }
            var target = await _context.Property
                .Include(m => m.User)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (target == null)
            {
                return RedirectToAction("NOTFOUND", "App");
            }
            var user = await _userManager.GetUserAsync(User);
            if (!(target.User == user || user.Role == MyLandUser.ROLE_MODERATOR))
            {
                return RedirectToAction("UNAUTHORIZED", "App");
            }
            target.IsActive = !target.IsActive;
            _context.Update(target);
            await _context.SaveChangesAsync();
            return RedirectToAction(GetIndexNameByUser());
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Destroy(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_MODERATOR)
            {
                return RedirectToAction("UNAUTHORIZED", "App");
            }
            if (id == null)
            {
                return RedirectToAction("NOTFOUND", "App");
            }
            var property = await _context.Property.FindAsync(id);
            if (property == null)
            {
                return RedirectToAction("NOTFOUND", "App");
            }
            await S3Service.DeleteImage(property.Photo);
            _context.Property.Remove(property);
            await _context.SaveChangesAsync();
            var snsService = new SNSService();
            await snsService.DeleteTopic("PropertyNotificationFor" + property.topicArn);
            return RedirectToAction(nameof(ModerateIndex));
        }

        // POST: Properties/Notify/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Notify(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NOTFOUND", "App");
            }
            var user = await _userManager.GetUserAsync(User);
            var property = await _context.Property.FindAsync(id);
            if (property == null)
            {
                return RedirectToAction("NOTFOUND", "App");
            }
            var notificationService = new NotificationService();
            await notificationService.CallLambda(
                property.Title,
                user.FirstName + " " + user.LastName,
                user.Email,
                user.Telephone.ToString(),
                property.topicArn);
            return Redirect(Request.Headers["Referer"].ToString());
        }

        private string GetIndexNameByUser()
        {
            var user = _userManager.GetUserAsync(User).Result;
            return user.Role switch
            {
                MyLandUser.ROLE_AGENT => nameof(AgentIndex),
                MyLandUser.ROLE_MODERATOR => nameof(ModerateIndex),
                _ => nameof(Index)
            };
        }
    }
}
