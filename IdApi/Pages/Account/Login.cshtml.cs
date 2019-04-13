using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdApi.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdApi.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public LoginModel(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
        }

        public string ReturnUrl { get; set; }

        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
        public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            // build a model so we know what to show on the login page
            await BuildLoginViewModelAsync(returnUrl);

            if (IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToPage("./LoginExternal", new { provider = ExternalLoginScheme, returnUrl });
            }

            return Page();
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new LoginInputModel();

        public async Task<IActionResult> OnPostAsync()
        {
            return Redirect(Input.ReturnUrl);
        }

        private async Task BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            if (context?.IdP != null)
            {
                // this is meant to short circuit the UI and only trigger the one external IdP
                EnableLocalLogin = false;
                ReturnUrl = returnUrl;
                Input.Username = context?.LoginHint;
                ExternalProviders = new ExternalProvider[] { new ExternalProvider { AuthenticationScheme = context.IdP } };

            }
            else
            {
                var schemes = await _schemeProvider.GetAllSchemesAsync();

                var providers = schemes
                    .Where(x => x.DisplayName != null ||
                                (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                    )
                    .Select(x => new ExternalProvider
                    {
                        DisplayName = x.DisplayName,
                        AuthenticationScheme = x.Name
                    }).ToList();

                var allowLocal = true;
                if (context?.ClientId != null)
                {
                    var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                    if (client != null)
                    {
                        allowLocal = client.EnableLocalLogin;

                        if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                        {
                            providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                        }
                    }
                }

                AllowRememberLogin = AccountOptions.AllowRememberLogin;
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin;
                Input.ReturnUrl = ReturnUrl = returnUrl;
                Input.Username = context?.LoginHint;
                ExternalProviders = providers.ToArray();
            }
        }
    }
}