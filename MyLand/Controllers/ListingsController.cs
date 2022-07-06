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
                    //TODO Change name to S3 image link
                    item.ListingPhoto = "~/imgs/"+item.ListingPhoto;

                    var user = await _userManager.FindByNameAsync(item.UserName);
                    if (user == null) { item.UserName = "Not Found"; }
                    else { item.UserName = user.UserFirstName + ' ' + user.UserLastName; }
                } else { item.UserName = "Not Found"; }
                if (item.ListingActive == 1)
                { filteredList.Add(item); }
            }
            return View(filteredList);
        }

        // GET: Moderate Listings
        public async Task<IActionResult> Manage()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                var listing = await _context.Listing.ToListAsync();
                List<MyLand.Models.Listing> filteredList = new List<MyLand.Models.Listing>();
                foreach (var item in listing)
                {
                    if (item.UserName == user.UserName)
                    { filteredList.Add(item); }
                }
                return View(filteredList);
            }
            return NotFound($"User login required");
        }

        // GET: Moderate Listings
        public async Task<IActionResult> Moderate()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (user.UserRole == 1)
                {
                    var listing = await _context.Listing.ToListAsync();
                    foreach (var item in listing)
                    {
                        if (item.UserName != null)
                        {
                            var listingUser = await _userManager.FindByNameAsync(item.UserName);
                            if (listingUser == null) { item.UserName = "Not Found"; }
                            else { item.UserName = listingUser.UserFirstName + ' ' + listingUser.UserLastName; }
                        }
                        else { item.UserName = "Not Found"; }
                    }
                    return View(listing);
                }
                return NotFound();
            }
            return NotFound($"User login required");
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }
            var listing = await _context.Listing
                .FirstOrDefaultAsync(m => m.ListingId == id);
            if (listing == null) { return NotFound(); }

            //TODO Change name to S3 image link
            listing.ListingPhoto = "~/imgs/" + listing.ListingPhoto;

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
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
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
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (id == null) { return NotFound(); }
                var targetListing = await _context.Listing.FindAsync(id);
                if (targetListing == null) { return NotFound(); }
                if (targetListing.UserName == user.UserName | user.UserRole == 1) { return View(targetListing); } 
                else { return NotFound($"Only listing owner may edit"); }
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
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (id == null) { return NotFound(); }
                var targetListing = await _context.Listing.AsNoTracking().FirstOrDefaultAsync(item => item.ListingId == id);
                if (targetListing == null) { return NotFound(); }
                if (targetListing.UserName == user.UserName | user.UserRole == 1) {
                    if (ModelState.IsValid) {
                        //readd some variable
                        listing.UserName = targetListing.UserName;
                        listing.ListingActive = targetListing.ListingActive;
                        try {
                            _context.Update(listing);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException) {
                            if (!ListingExists(listing.ListingId)) { return NotFound(); }
                            else { return NotFound($"An error has occured"); }
                        }
                        return RedirectToAction(nameof(Manage));
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
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (id == null) { return NotFound(); }
                var targetListing = await _context.Listing.FindAsync(id);
                if (targetListing == null) { return NotFound(); }
                if (user.UserRole == 1)
                { return View(targetListing); }
                else
                { return NotFound($"Only admin may delete"); }
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
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                if (id == null) { return NotFound(); }
                var targetListing = await _context.Listing.FindAsync(id);
                if (targetListing == null) { return NotFound(); }
                if ( user.UserRole == 1)
                {
                    _context.Listing.Remove(targetListing);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Moderate));
                }
                else
                { return NotFound($"Only admin may delete"); }
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
                    targetListing.ListingActive = 0;
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

        // POST: Listings/Notify/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Notify(int? id)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var targetListing = await _context.Listing.FindAsync(id);
                if (targetListing == null) { return NotFound(); }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                { return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); }
                var listingUser = await _userManager.FindByNameAsync(targetListing.UserName);
                if (user == null)
                { return NotFound($"Listing poster not found"); }
                //TODO SNS
                //user.UserEmail
                //listingUser.UserEmail
                return NotFound($"This function will send email");
            }
            return NotFound($"User login required");
        }

        private bool ListingExists(int id)
        {
            return _context.Listing.Any(e => e.ListingId == id);
        }
    }
}
