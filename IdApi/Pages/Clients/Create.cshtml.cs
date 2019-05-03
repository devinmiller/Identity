using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdApi.Models.Identity;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdApi.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly ConfigurationDbContext _context;


        public void OnGet()
        {
            Input = new ClientInputModel();
        }

        [BindProperty]
        public ClientInputModel Input { get; set; }
    }
}