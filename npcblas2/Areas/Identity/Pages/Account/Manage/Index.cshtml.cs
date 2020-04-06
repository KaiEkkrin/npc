using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using npcblas2.Data;

namespace npcblas2.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RandomNumberGenerator _rng;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RandomNumberGenerator rng)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _rng = rng;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Public handle")]
            [MaxLength(20)]
            public string Handle { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;

            Input = new InputModel
            {
                Handle = user.Handle
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

            string uniqueHandle;
            if (Input.Handle != user.Handle && (uniqueHandle = SuggestUniqueHandle(Input.Handle)) != null)
            {
                user.Handle = uniqueHandle;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        private string SuggestUniqueHandle(string inputHandle)
        {
            // We add a random suffix after the #
            var baseHandle = inputHandle?.Split('#', StringSplitOptions.RemoveEmptyEntries)?.FirstOrDefault()?.Trim();
            if (baseHandle == null)
            {
                return null;
            }

            // This should be enough:
            var data = new byte[4];
            _rng.GetBytes(data);
            var randomSuffix = Convert.ToBase64String(data).TrimEnd('=');
            return $"{baseHandle}#{randomSuffix}";
        }
    }
}
