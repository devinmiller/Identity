﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdApi.Pages.Account
{
    public class RedirectModel : PageModel
    {
        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl)
        {
            
        }
    }
}