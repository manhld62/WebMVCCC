using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RaZorWebxxx.Models;

namespace RaZorWebxxx.Areas.Admins.Pages.User
{
    [Authorize(Roles = "admin")]
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyBlogContext _context;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
             MyBlogContext context                )
        { 
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]


        [TempData]
        public string StatusMessage { get; set; }
        public  SelectList  allRoles { set; get; }


        public AppUser user { set; get; }
        [BindProperty]
        [DisplayName ("Các Role gán cho user")]
        public string [] RolesNames { set; get; }
        public List<IdentityRoleClaim<string>> claimsInRole { set; get; }
        public List<IdentityUserClaim<string>> claimsInUserClaim { set; get; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"khong co user");
            }
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("khong thay user");
            }
            RolesNames= (await _userManager.GetRolesAsync(user)).ToArray<string>();
            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
    
      

             allRoles = new SelectList(roleNames);
            await getClaims(id);


            return Page();
        }
        async Task getClaims(string id)
        {
            var listRoles = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;
            var _claimsInRole = from c in _context.RoleClaims
                                join r in listRoles on c.RoleId equals r.Id
                                select c;
            claimsInRole = await _claimsInRole.ToListAsync();
            claimsInUserClaim= await (from c in _context.UserClaims
             where c.UserId == id
             select c).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Không có user");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($" Không tim thấy user ");
            }
            var oldrolename = (await _userManager.GetRolesAsync(user)).ToArray();
            var deleteroles = oldrolename.Where(r => !RolesNames.Contains(r));
            var addroles = RolesNames.Where(r => !oldrolename.Contains(r));
            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();



            allRoles = new SelectList(roleNames);
            await getClaims(id);
          
            var resultdelete = await _userManager.RemoveFromRolesAsync(user, deleteroles);
            if (!resultdelete.Succeeded)
            {
                resultdelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);

                });
                return Page();
            }
            
            var resultAdd = await _userManager.AddToRolesAsync(user, addroles);
            if (!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);

                });
                return Page();
            }

          

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = $"Cập Nhật Roles Thành Công cho user :{user.UserName}";

            return RedirectToPage();
        }
    }
}
