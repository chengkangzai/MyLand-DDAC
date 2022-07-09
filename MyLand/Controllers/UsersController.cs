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
    public class UsersController : Controller
    {
        private readonly UserManager<MyLandUser> _userManager;
        private readonly SignInManager<MyLandUser> _signInManager;
        public UsersController(
            UserManager<MyLandUser> userManager, 
            SignInManager<MyLandUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (user.Role == 1)
                {
                    var users = await _userManager.Users.ToListAsync();
                    foreach (var item in users)
                    {
                        if (item.Role == 1) { user.Address = "Admin"; }
                        else { user.Address = "User"; }
                    }
                    return View(users);
                }
                else
                {
                    return NotFound($"Only admin may delete");
                }
            }
            return NotFound($"User login required");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string username)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (user.Role == 1)
                {
                    if (username == null) { return NotFound(); }
                    var targetUser = await _userManager.FindByNameAsync(username);
                    if (targetUser == null) { return NotFound(); }
                    await _userManager.DeleteAsync(targetUser);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound($"Only admin may delete");
                }
            }
            return NotFound($"User login required");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeUser(string username)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (user.Role == 1)
                {
                    if (username == null) { return NotFound(); }
                    var targetUser = await _userManager.FindByNameAsync(username);
                    if (targetUser == null) { return NotFound(); }
                    targetUser.Role = 0;
                    await _userManager.UpdateAsync(targetUser);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound($"Only admin may change role");
                }
            }
            return NotFound($"User login required");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string username)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                if (user.Role == 1)
                {
                    if (username == null) { return NotFound(); }
                    var targetUser = await _userManager.FindByNameAsync(username);
                    if (targetUser == null) { return NotFound(); }
                    targetUser.Role = 1;
                    await _userManager.UpdateAsync(targetUser);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound($"Only admin may change role");
                }
            }
            return NotFound($"User login required");
        }
    }
}
