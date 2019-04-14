using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdApi.Models;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdApi.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;

        public LogoutModel(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

        [BindProperty(SupportsGet = true)]
        public string LogoutId { get; set; }

        public bool ShowLogoutPrompt { get; set; }

        public async Task<IActionResult> OnGet()
        {
            await BuildLogoutViewModelAsync(LogoutId);

            if (ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return OnPost();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("./LoggedOut", new { logoutId = LogoutId });
        }

        private async Task BuildLogoutViewModelAsync(string logoutId)
        {
            LogoutId = logoutId;
            ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt;

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                ShowLogoutPrompt = false;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);

            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                ShowLogoutPrompt = false;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
        }
    }
}