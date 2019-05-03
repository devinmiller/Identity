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

        [Display(Name = "Redirect URIs")]
        public List<string> RedirectUris { get; set; }

        [Display(Name = "Logout URIs")]
        public List<string> PostLogoutRedirectUris { get; set; }

        [Display(Name = "CORS Origins")]
        public List<string> AllowedCorsOrigins { get; set; }
    }
}
