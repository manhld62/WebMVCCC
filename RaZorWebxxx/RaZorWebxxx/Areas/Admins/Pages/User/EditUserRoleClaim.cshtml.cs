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

namespace RaZorWebxxx.Areas.Admins.Pages.User
{
    [Authorize(Roles = "admin")]
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EditUserRoleClaimModel(MyBlogContext myBlogContext,UserManager<AppUser> userManager)
        {
            _context = myBlogContext;
            _userManager = userManager;

        }
        [TempData]
        public string StatuMessage { set; get; }
        public NotFoundObjectResult OnGet() => NotFound("Không được truy cập");
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
        public AppUser user { set; get; }
        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("Không tìm thấy user");
            return Page();
        }
        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("Không tìm thấy user");
            if(!ModelState.IsValid)
              return Page();
            var claims = _context.UserClaims.Where(c => c.UserId == user.Id);
            if(claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Đặc Tính Này đã có");
                return Page();
            }
         await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
            StatuMessage = "Đã Thêm đặc tính cho User";
            return RedirectToPage("./AddRole", new { Id = user.Id });
        }
        public IdentityUserClaim<string> userClaim { set; get; }
        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid)
        {
            
            if (claimid == null) return NotFound("Không tìm thấy user");
            userClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null) return NotFound("Không tìm thấy user");
            Input = new InputModel
            {
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue
            };
            return Page();
        }
        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid)
        {

            if (claimid == null) return NotFound("Không tìm thấy user");
            userClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null) return NotFound("Không tìm thấy user");
            if (!ModelState.IsValid) return Page();
          if(  _context.UserClaims.Any(c=>c.UserId==user.Id && c.ClaimType== Input.ClaimType&& c.ClaimValue==Input.ClaimValue && c.Id !=userClaim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có");
                return Page();
            }
                userClaim.ClaimType = Input.ClaimType;
            userClaim.ClaimValue = Input.ClaimValue;
            await _context.SaveChangesAsync();
            StatuMessage = "Bạn vừa cập nhật Claim";

            return RedirectToPage("./AddRole", new { Id = user.Id });
        }
        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {

            if (claimid == null) return NotFound("Không tìm thấy user");
            userClaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if (user == null) return NotFound("Không tìm thấy user");
            await _userManager.RemoveClaimAsync(user, new Claim(userClaim.ClaimType,userClaim.ClaimValue));
           
           
            
           
            StatuMessage = "Bạn đã xóa claim ";

            return RedirectToPage("./AddRole", new { Id = user.Id });
        }
    }
}
