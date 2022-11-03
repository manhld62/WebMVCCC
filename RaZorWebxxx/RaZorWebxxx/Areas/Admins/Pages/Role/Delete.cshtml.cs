using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RaZorWebxxx.Models;

namespace RaZorWebxxx.Areas.Admins.Pages.Role
{
    [Authorize(Roles = "admin")]
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, MyBlogContext myblogcontext) : base(roleManager, myblogcontext)
        {
        }
        public IdentityRole role { set; get; }




        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null) return NotFound("Không Tìm Thấy role");
             role = await _roleManager.FindByIdAsync(roleid);
            if(role == null)
            {
                return NotFound("Không tìm thấy role");
            }
            return Page();
           
        }
        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null) return NotFound("khong tim thay role");
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null) return NotFound("khong tim thay role");
      
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                Statusmessage = $"Bạn vừa xóa: {role.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }
    }
}
