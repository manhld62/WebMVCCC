using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RaZorWebxxx.Models;
using RaZorWebxxx.Models.Product;

namespace RaZorWebxxx.Areas.Products
{
    [Area("Product")]
    [Route("admin/categoryproduct/caregory/[action]/{id?}")]
    [Authorize(Roles = "admin")]
    public class CategoryproductController : Controller
    {
        private readonly MyBlogContext _context;

        public CategoryproductController(MyBlogContext context)
        {
            _context = context;
        }

        // GET: Blog/Categories
        public async Task<IActionResult> Index()
        {
            var qr = (from c in _context.Categoryproducts select c).Include(c => c.ParentCategory).Include(c => c.CategoryChildren);
            var categories = (await qr.ToListAsync()).Where(c => c.ParentCategory == null).ToList();
            return View(categories);
        }

        // GET: Blog/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categoryproducts
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        private void CreateItems(List<Categoryproduct> source,List<Categoryproduct>des,int level)
        {
            foreach(var category in source)
            {
                des.Add(category);
                if (category.CategoryChildren?.Count > 0)
                {
                    CreateItems(category.CategoryChildren.ToList(), des, level + 1);
                }
            }
            
           

        }

        // GET: Blog/Categories/Create
        public async Task <IActionResult> CreateAsync()
        {
            var qr = (from c in _context.Categoryproducts select c).Include(c => c.ParentCategory).Include(c => c.CategoryChildren);
            var categori = (await qr.ToListAsync()).ToList();
            categori.Insert(0, new Categoryproduct()
            {
                Id = -1,
                Title = "Không có danh mục cha"

            });
            var items = new List<Categoryproduct>();
            CreateItems(categori, items, 0);

            var selectLists = new SelectList(categori, "Id", "Title");
           
            

            ViewData["ParentCategoryId"] = selectLists;
            return View();
        }

        // POST: Blog/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentCategoryId,Title,Description,Slug")] Categoryproduct category)
        {

          
            if (ModelState.IsValid)
            {
                if (category.ParentCategoryId == -1) category.ParentCategoryId = null;
                    _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var qr = (from c in _context.Categoryproducts select c).Include(c => c.ParentCategory).Include(c => c.CategoryChildren);
            var categori = (await qr.ToListAsync()).Where(c => c.ParentCategory == null).ToList();
            categori.Insert(0, new Categoryproduct()
            {
                Id = -1,
                Title = "Không có danh mục cha"

            });
            
            var selectLists = new SelectList(categori, "Id", "Title");


            ViewData["ParentCategoryId"] = selectLists;
            return View(category);
        }

        // GET: Blog/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var qr = (from c in _context.Categoryproducts select c).Include(c => c.ParentCategory).Include(c => c.CategoryChildren);
            var categori = (await qr.ToListAsync()).ToList();
            categori.Insert(0, new Categoryproduct()
            {
                Id = -1,
                Title = "Không có danh mục cha"

            });
            ;
            var selectLists = new SelectList(categori, "Id", "Title");

            var category = await _context.Categoryproducts.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentCategoryId"] = selectLists;
            return View(category);
        }

        // POST: Blog/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentCategoryId,Title,Description,Slug")] Categoryproduct category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentCategoryId == -1)
                    category.ParentCategoryId = null;
                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var qr = (from c in _context.Categories select c).Include(c => c.ParentCategory).Include(c => c.CategoryChildren);
            var categori = (await qr.ToListAsync()).ToList();
            categori.Insert(0, new Category()
            {
                Id = -1,
                Title = "Không có danh mục cha"

            });
            var selectLists = new SelectList(categori, "Id", "Title");
            ViewData["ParentCategoryId"] = selectLists;
            return View(category);
        }

        // POST: Blog/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categoryproducts.Include(c => c.CategoryChildren).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            
            foreach(var categorry in category.CategoryChildren)
            {
                categorry.ParentCategoryId = category.ParentCategoryId;
            }
            
            _context.Categoryproducts.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categoryproducts.Any(e => e.Id == id);
        }
    }
}
