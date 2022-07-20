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


namespace MyLand.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<MyLandUser> _userManager;

        public UsersController(UserManager<MyLandUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_ADMIN)
            {
                return RedirectToAction("Handle403", "AuthException");
            }
            var users = await _userManager.Users.ToListAsync();
            foreach (var item in users)
            {
                user.Address = item.Role == MyLandUser.ROLE_ADMIN ? "Admin" : "User";
            }
            return View(users);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string username)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_ADMIN)
            {
                return RedirectToAction("Handle403", "AuthException");
            }
            if (username == null)
            {
                return RedirectToAction("Handle404", "AuthException");
            }
            var targetUser = await _userManager.FindByNameAsync(username);
            if (targetUser == null)
            {
                return RedirectToAction("Handle404", "AuthException");
            }
            await _userManager.DeleteAsync(targetUser);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeUser(string username)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_ADMIN)
            {
                return RedirectToAction("Handle403", "AuthException");
            }
            if (username == null)
            {
                return RedirectToAction("Handle404", "AuthException");
            }
            var targetUser = await _userManager.FindByNameAsync(username);
            if (targetUser == null)
            {
                return RedirectToAction("Handle404", "AuthException");
            }
            targetUser.Role = MyLandUser.ROLE_USER;
            await _userManager.UpdateAsync(targetUser);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string username)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_ADMIN)
            {
                return RedirectToAction("Handle403", "AuthException");
            }

            if (username == null)
            {
                return RedirectToAction("Handle404", "AuthException");
            }
            var targetUser = await _userManager.FindByNameAsync(username);
            if (targetUser == null)
            {
                return RedirectToAction("Handle404", "AuthException");
            }
            targetUser.Role = MyLandUser.ROLE_ADMIN;
            await _userManager.UpdateAsync(targetUser);
            return RedirectToAction(nameof(Index));
        }
    }
}
