using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RaZorWebxxx.Models;

namespace RaZorWebxxx.Areas.Admins.Pages.Role
{
    [Authorize(Roles = "admin")]
    public class AddRoleClaimModel : RolePageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext myblogcontext) : base(roleManager, myblogcontext)
        {
        }
        public class InputModel {

            [Display(Name ="Tên  Claim")]
            [Required(ErrorMessage ="Phải Nhập {0}")]
            [StringLength(256,MinimumLength =3,ErrorMessage ="{0} phải dài {2} đến {1} kí tự")]
            public string ClaimType { set; get; }

            [Display(Name = "Giá Trị")]
            [Required(ErrorMessage = "Phải Nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài {2} đến {1} kí tự")]
            public string ClaimValue { set; get; }

        }
        [BindProperty]
        public InputModel Input { set; get; }
        public IdentityRole role { set; get; }


        public async Task<IActionResult> OnGet(string roleid)
        {
             role = await _roleManager.FindByIdAsync( roleid);
            if (role == null) return NotFound("Không Tìm thấy role");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null) return NotFound("Khong tim thay role");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if ((await _roleManager.GetClaimsAsync(role)).Any(c => c.Type == Input.ClaimType && c.Value == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong ROLE");
                return Page();
            }
            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result = await _roleManager.AddClaimAsync(role, newClaim);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e =>
                {
                    ModelState.AddModelError(string.Empty, e.Description);
                });
                return Page();
            }
            Statusmessage = "Vừa Thêm đặc tính claim mới";

            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
    }
}
