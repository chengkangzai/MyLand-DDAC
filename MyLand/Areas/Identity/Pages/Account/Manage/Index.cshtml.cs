using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyLand.Areas.Identity.Data;

namespace MyLand.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<MyLandUser> _userManager;
        private readonly SignInManager<MyLandUser> _signInManager;

        public IndexModel(
            UserManager<MyLandUser> userManager,
            SignInManager<MyLandUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "First name cannot be empty", MinimumLength = 1)]
            [Display(Name = "First Name")]
            public string UserFirstName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Last name cannot be empty", MinimumLength = 1)]
            [Display(Name = "Last Name")]
            public string UserLastName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Address cannot be empty", MinimumLength = 1)]
            [Display(Name = "Address")]
            public string UserAddress { get; set; }

            [Required]
            [Display(Name = "Telephone")]
            public int UserTelephone { get; set; }
        }

        private async Task LoadAsync(MyLandUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;

            Input = new InputModel
            {
                UserFirstName = user.UserFirstName,
                UserLastName = user.UserLastName,
                UserAddress = user.UserAddress,
                UserTelephone = user.UserTelephone,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.UserFirstName != user.UserFirstName) { user.UserFirstName = Input.UserFirstName; }
            if (Input.UserLastName != user.UserLastName) { user.UserLastName = Input.UserLastName; }
            if (Input.UserAddress != user.UserAddress) { user.UserAddress = Input.UserAddress; }
            if (Input.UserTelephone != user.UserTelephone) { user.UserTelephone = Input.UserTelephone; }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
