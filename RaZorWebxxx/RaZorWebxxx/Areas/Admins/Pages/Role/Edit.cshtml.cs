using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RaZorWebxxx.Models;

namespace RaZorWebxxx.Areas.Admins.Pages.Role
{ 
    
    //[Authorize(Policy = "AllowEditRole")]
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, MyBlogContext myblogcontext) : base(roleManager, myblogcontext)
        {
        }
        public class InputModel
        {

            [Display(Name = "tên của role")]
            [Required(ErrorMessage = "Phải Nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài {2} đến {1} kí tự")]
            public string Name { set; get; }


        }
        [BindProperty]
        public InputModel Input { set; get; }
        public IdentityRole role { set; get; }
        public List<IdentityRoleClaim<string>> Claims { set; get; }

        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role ");
             role = await _roleManager.FindByIdAsync(roleid);
            if (role != null)
            {
                Input = new InputModel()
                {
                    Name = role.Name
                };
                Claims = await _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();
                return Page();


            }
            return NotFound("Không tìm thấy role ");
           
        }
        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null) return NotFound("khong tim thay role");
            role = await _roleManager.FindByIdAsync(roleid);
            if(role==null) return NotFound("khong tim thay role");
            Claims = await _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(role);
           
            
            if (result.Succeeded)
            {
                Statusmessage = $"Bạn vừa tạo đổi tên : {Input.Name}";
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
