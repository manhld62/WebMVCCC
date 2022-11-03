using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RaZorWebxxx.Models;
using Microsoft.AspNetCore.Mvc;

namespace RaZorWebxxx.Areas.Admins.Pages.Role
{
    public class RolePageModel: PageModel
    {
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly MyBlogContext _context;
        [TempData]
        public string Statusmessage { set; get; }
        public RolePageModel(RoleManager<IdentityRole> roleManager,MyBlogContext myblogcontext)
        {
            _roleManager = roleManager;
            _context = myblogcontext;


        }
    }
}
