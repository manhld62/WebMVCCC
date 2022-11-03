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
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext myblogcontext) : base(roleManager, myblogcontext)
        {
        }
        public class InputModel
        {

            [Display(Name = "Tên  Claim")]
            [Required(ErrorMessage = "Phải Nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài {2} đến {1} kí tự")]
            public string ClaimType { set; get; }

            [Display(Name = "Giá Trị")]
            [Required(ErrorMessage = "Phải Nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài {2} đến {1} kí tự")]
            public string ClaimValue { set; get; }

        }
        [BindProperty]
        public InputModel Input { set; get; }
        public IdentityRole role { set; get; }
        IdentityRoleClaim<string> claim { set; get; }


        public async Task<IActionResult> OnGet(int? claimid)
        {
            if (claimid == null) return NotFound("Không tìm thấy role");
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound(" khong tim thay role");
            Input = new InputModel() { ClaimType = claim.ClaimType, ClaimValue = claim.ClaimValue };

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? claimid)
        {

            if (claimid == null) return NotFound("Không tìm thấy role");
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound(" khong tim thay role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

                if ((_context.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id)))
                {
                    ModelState.AddModelError(string.Empty, "Claim này đã có trong ROLE");
                    return Page();
                }
                claim.ClaimType = Input.ClaimType;
                claim.ClaimValue = Input.ClaimValue;
                await _context.SaveChangesAsync();

                Statusmessage = "Vừa cập nhật claim";

                return RedirectToPage("./Edit", new { roleid = role.Id });
            
        }
        public async Task<IActionResult> OnPostDelete(int? claimid)
        {

            if (claimid == null) return NotFound("Không tìm thấy role");
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");
            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound(" khong tim thay role");




            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));

            Statusmessage = "Vừa xóa claim ";

            return RedirectToPage("./Edit", new { roleid = role.Id });

        }
    }
}
