using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdApi.Models.Identity
{
    public class ClientInputModel
    {
        [Display(Name = "Client Id")]
        public string ClientId { get; set; }
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }
        [Display(Name = "Client URI")]
        public string ClientUri { get; set; }

        [Display(Name = "Login URI")]
        public string RedirectUri { get; set; }
        [Display(Name = "Logout URI")]
        public string PostLogoutRedirectUri { get; set; }
        [Display(Name = "CORS Origin")]
        public string AllowedCorsOrigin { get; set; }

        public GrantTypes GrantType { get; set; }
    }
}
