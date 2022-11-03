using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaZorWebxxx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaZorWebxxx.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DBManageController : Controller
    {
        private readonly MyBlogContext _dbContext;
        public DBManageController(MyBlogContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }
        [TempData]
        public string StatusMessage { set; get; }
        [HttpPost]
        public async Task  <IActionResult> DeletedbAsync()
        {
            var success = await _dbContext.Database.EnsureDeletedAsync();
            StatusMessage = success ? "Xóa Db thành công " : "Không xóa đc ";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _dbContext.Database.MigrateAsync();
            StatusMessage = "Cập Nhật Databse thành công";
            return RedirectToAction(nameof(Index));
        }


    }
}
