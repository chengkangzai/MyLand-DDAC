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


namespace MyLand.Views.Listing
{
    public class ListingsController : Controller
    {
        private readonly MyLandListingContext _context;
        private readonly UserManager<MyLandUser> _userManager;
        private readonly SignInManager<MyLandUser> _signInManager;
        public ListingsController(MyLandListingContext context, 
            UserManager<MyLandUser> userManager, 
            SignInManager<MyLandUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Listings
        public async Task<IActionResult> Index()
        {
            var listing = await _context.Listing.ToListAsync();
            List<MyLand.Models.Listing> filteredList = new List<MyLand.Models.Listing>();
            foreach (var item in listing){
                if(item.UserName != null){
                    var user = await _userManager.FindByNameAsync(item.UserName);
                    if (user == null) { item.UserName = "Not Found"; }
                    else { item.UserName = user.UserFirstName + ' ' + user.UserLastName; }
                } else { item.UserName = "Not Found"; }
                if (item.ListingActive == 0)
                {
                    filteredList.Add(item);
                }
            }
            return View(filteredList);
        }

        // GET: Own Listings
        public async Task<IActionResult> Manage()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                var listing = await _context.Listing.ToListAsync();
                List<MyLand.Models.Listing> filteredList = new List<MyLand.Models.Listing>();
                foreach (var item in listing)
                {
                    if (item.UserName == user.UserName)
                    {
                        filteredList.Add(item);
                    }
                }
                return View(filteredList);
            }
            return NotFound($"User login required");
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _context.Listing
                .FirstOrDefaultAsync(m => m.ListingId == id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // GET: Listings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ListingId,ListingType,ListingTitle,ListingDescription,ListingPrice,ListingSize,ListingPhoto,ListingDate")] Models.Listing listing)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                listing.UserName = user.UserName;
                listing.ListingActive = 1;
                if (ModelState.IsValid)
                {
                    _context.Add(listing);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(listing);
            }
            return NotFound($"User login required");
        }

        // GET: Listings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (id == null) { return NotFound(); }
                var listing = await _context.Listing.FindAsync(id);
                if (listing == null) { return NotFound(); }
                if (listing.UserName == user.UserName | user.UserRole == 1)
                {
                    return View(listing);
                } else
                {
                    return NotFound($"Only listing owner may edit");
                }
            }
            return NotFound($"User login required");
            
        }

        // POST: Listings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("ListingId,ListingType,ListingTitle,ListingDescription,ListingPrice,ListingSize,ListingPhoto,ListingDate")] Models.Listing listing)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (id == null) { return NotFound(); }
                var targetListing = await _context.Listing.FindAsync(id);
                if (targetListing == null) { return NotFound(); }
                if (targetListing.UserName == user.UserName | user.UserRole == 1)
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(listing);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!ListingExists(listing.ListingId))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    return View(listing);
                }
                else
                {
                    return NotFound($"Only listing owner may edit");
                }
            }
            return NotFound($"User login required");
        }

        // GET: Listings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (id == null) { return NotFound(); }
                var targetListing = await _context.Listing.FindAsync(id);
                if (targetListing == null) { return NotFound(); }
                if (user.UserRole == 1)
                {
                    return View(targetListing);
                }
                else
                {
                    return NotFound($"Only admin may delete");
                }
            }
            return NotFound($"User login required");
        }

        // POST: Listings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (id == null) { return NotFound(); }
                var targetListing = await _context.Listing.FindAsync(id);
                if (targetListing == null) { return NotFound(); }
                if ( user.UserRole == 1)
                {
                    var listing = await _context.Listing.FindAsync(id);
                    _context.Listing.Remove(listing);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound($"Only admin may delete");
                }
            }
            return NotFound($"User login required");
        }

        // POST: Listings/Inactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (id == null) { return NotFound(); }
                var targetListing = await _context.Listing.FindAsync(id);
                if (targetListing == null) { return NotFound(); }
                if (user.UserRole == 1)
                {
                    var listing = await _context.Listing.FindAsync(id);
                    listing.ListingActive = 0;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Manage));
                }
                else
                {
                    return NotFound($"Only admin may delete");
                }
            }
            return NotFound($"User login required");
        }

        private bool ListingExists(int id)
        {
            return _context.Listing.Any(e => e.ListingId == id);
        }
    }
}
