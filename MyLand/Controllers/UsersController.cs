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
            if (user.Role != MyLandUser.ROLE_MODERATOR)
            {
                return Unauthorized();
            }
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Show(string id)
        {
            if (id == "")
            {
                return RedirectToAction("NotFound", "App");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_MODERATOR)
            {
                return RedirectToAction("Unauthorized", "App");
            }
            var userToShow = await _userManager.FindByIdAsync(id);
            if (userToShow == null)
            {
                return RedirectToAction("NotFound", "App");
            }
            return View(userToShow);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_MODERATOR)
            {
                return RedirectToAction("Unauthorized", "App");
            }
            var userToEdit = await _userManager.FindByIdAsync(id);
            if (userToEdit == null)
            {
                return RedirectToAction("NotFound", "App");
            }

            return View(userToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole([FromForm] string id, [FromForm] int role)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_MODERATOR)
            {
                return RedirectToAction("Unauthorized", "App");
            }
            if (id == null)
            {
                return RedirectToAction("NotFound", "App");
            }
            var targetUser = await _userManager.FindByIdAsync(id);
            if (targetUser == null)
            {
                return RedirectToAction("NotFound", "App");
            }
            targetUser.Role = role;
            await _userManager.UpdateAsync(targetUser);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] string username)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Role != MyLandUser.ROLE_MODERATOR)
            {
                return RedirectToAction("Unauthorized", "App");
            }
            if (username == null)
            {
                return RedirectToAction("NotFound", "App");
            }
            var targetUser = await _userManager.FindByIdAsync(username);
            if (targetUser == null)
            {
                return RedirectToAction("NotFound", "App");
            }
            await _userManager.DeleteAsync(targetUser);
            return RedirectToAction(nameof(Index));
        }
    }
}
