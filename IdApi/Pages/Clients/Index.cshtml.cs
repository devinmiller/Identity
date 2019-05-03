using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdApi.Models.Identity;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdApi.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly ConfigurationDbContext _context;

        public IndexModel(ConfigurationDbContext context)
        {
            _context = context;
        }

        public List<ClientsViewModel> Clients { get; set; }

        public async Task OnGetAsync()
        {
            Clients = new List<ClientsViewModel>();
        }
    }
}