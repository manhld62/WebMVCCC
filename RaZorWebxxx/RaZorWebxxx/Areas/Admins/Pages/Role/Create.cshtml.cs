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
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, MyBlogContext myblogcontext) : base(roleManager, myblogcontext)
        {
        }
        public class InputModel {

            [Display(Name ="tên của role")]
            [Required(ErrorMessage ="Phải Nhập {0}")]
            [StringLength(256,MinimumLength =3,ErrorMessage ="{0} phải dài {2} đến {1} kí tự")]
            public string Name { set; get; }
  
        
        }
        [BindProperty]
        public InputModel Input { set; get; }


        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var newRole = new IdentityRole(Input.Name);
            var result=   await _roleManager.CreateAsync(newRole);
            if (result.Succeeded)
            {
                Statusmessage = $"Bạn vừa tạo role mới : {Input.Name}";
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
