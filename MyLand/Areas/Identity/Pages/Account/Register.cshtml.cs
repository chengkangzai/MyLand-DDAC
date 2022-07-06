using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MyLand.Areas.Identity.Data;

namespace MyLand.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<MyLandUser> _signInManager;
        private readonly UserManager<MyLandUser> _userManager;

        public RegisterModel(
            UserManager<MyLandUser> userManager,
            SignInManager<MyLandUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

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

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userRole = 0;
            if (Input.Email == "admin@myland.com")
            {
                userRole = 1;
            }
            var user = new MyLandUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                UserFirstName = Input.UserFirstName,
                UserLastName = Input.UserLastName,
                UserAddress = Input.UserAddress,
                UserTelephone = Input.UserTelephone,
                UserRole = userRole
            };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
