using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdApi.Models.Identity
{
    public class ClientsViewModel
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public string AllowedGrantTypes { get; set; }
        public bool RequireClientSecret { get; set; }
        public bool RequireConsent { get; set; }
        public bool RequirePkce { get; set; }
        public bool Enabled { get; set; }
    }
}
