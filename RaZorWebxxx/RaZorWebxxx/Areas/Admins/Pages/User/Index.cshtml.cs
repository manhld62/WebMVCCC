using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaZorWebxxx.Models;
namespace RaZorWebxxx.Areas.Admins.Pages.User
{
    [Authorize]

    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager) 
        {
            _userManager = userManager;

        }
        [TempData]
        public string Statusmessage { set; get; }
        public const int Itempage = 10;
        [BindProperty(SupportsGet =true,Name ="p")]
        public int currentpage { set; get; }
        public int countpage { set; get; }
        public List<UserAndRoles> users { set; get; }
        public int totalUsers { set; get; }
        public class UserAndRoles : AppUser { 

            public string Rolenames { set; get; }
        
        
        }

        public async Task OnGet()
        {
            //user = await  _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
          var qr=  _userManager.Users.OrderBy(u => u.UserName);
             totalUsers = await qr.CountAsync();
           
            countpage = (int)Math.Ceiling((double)totalUsers / Itempage);
            if (currentpage < 1)
                currentpage = 1;
            if (currentpage > countpage)
                currentpage = countpage;


            var qr1 =qr.Skip((currentpage - 1) * Itempage)
                   .Take(Itempage).Select(u =>new UserAndRoles()
                   {
                       Id=u.Id,
                       UserName=u.UserName,
                   })
                   ;
            users = await qr1.ToListAsync();
            foreach(var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Rolenames = string.Join(",", roles);
            }
        }
        public void OnPost() => RedirectToPage();
    }


}


