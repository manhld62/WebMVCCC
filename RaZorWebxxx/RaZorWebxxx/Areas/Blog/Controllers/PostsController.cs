﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RaZorWebxxx.AppUlti;
using RaZorWebxxx.Areas.Blog.Models;
using RaZorWebxxx.Models;
using RaZorWebxxx.Models.Blog;
using XTL.Helpers;

namespace RaZorWebxxx.Areas.Blog
{
    [Area("Blog")]
    [Route("admin/blog/post/[action]/{id?}")]
    [Authorize(Roles = "admin")]
    public class PostsController : Controller
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<AppUser> _userManager;


        public PostsController(MyBlogContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { set; get; }

        // GET: Blog/Posts
        public async Task<IActionResult> Index([FromQuery(Name="p")]int currentpagee,int pagesize)
        {
            var post = _context.Posts.Include(p => p.Author).OrderByDescending(p => p.DateUpdated);
            int totalpost = await post.CountAsync();
            if (pagesize <= 0) pagesize = 10;
            int countPages=(int)Math.Ceiling((double)totalpost/pagesize);
            if (currentpagee> countPages) currentpagee = countPages;
            if (currentpagee < 1) currentpagee = 1;
            var pagingModel = new PagingModel()
            {
                countpage = countPages,
                currentpage=currentpagee,
                generateUrl=(pageNumber) => Url.Action("Index", new
                {
                    p=pageNumber,
                    pagesize = pagesize
                })

            };
            ViewBag.pagingModel = pagingModel;
            ViewBag.totalPosts = totalpost;
            ViewBag.postIndex = (currentpagee - 1) * pagesize;

            var postsInPage = await post.Skip((currentpagee - 1) * pagesize).Take(pagesize).Include(p => p.PostCategories)
                .ThenInclude(pc => pc.Category).ToListAsync();
            return View(postsInPage);
        }

        // GET: Blog/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Blog/Posts/Create
        public async Task<IActionResult> CreateAsync()
        {
            var categories = await _context.Categories.ToListAsync();
           ViewData["categories"] =new MultiSelectList(categories, "Id", "Title");

            return View();
        }

        // POST: Blog/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIDs")] CreatePostModel post)
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            if (post.Slug == null)
            {
                post.Slug = Ulti.GenerateSlug(post.Title);
            }
            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(post);
            }
           
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(this.User);
                post.DateCreated = post.DateUpdated = DateTime.Now;
                post.AuthorId = user.Id;
                _context.Add(post);
                if (post.CategoryIDs != null)
                {
                    foreach(var CateId in post.CategoryIDs)
                    {
                        _context.Add(new Postcategory()
                        {
                            CategoryID = CateId,
                            Post=post
                        });
                        
                    }
                }
                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo bài viết mới";
                return RedirectToAction(nameof(Index));
            }

            return View(post);
        }

        // GET: Blog/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            
          
            var postEdit = new CreatePostModel()
            {
                PostId = post.PostId,
                Title = post.Title,
                Content=post.Content,
                Description=post.Description,
                Slug=post.Slug,
                Published=post.Published,
                CategoryIDs=post.PostCategories.Select(pc =>pc.CategoryID).ToArray()



            };
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            return View(postEdit);
        }

        // POST: Blog/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Description,Slug,Content,Published,CategoryIDs")] CreatePostModel post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            if (post.Slug == null)
            {
                post.Slug = Ulti.GenerateSlug(post.Title);
            }
            if (await _context.Posts.AnyAsync(p => p.Slug == post.Slug &&p.PostId !=id))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(post);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var postupdate = await _context.Posts.Include(p => p.PostCategories).FirstOrDefaultAsync(p => p.PostId == id);
                    if (postupdate==null)
                    {
                        return NotFound();
                    }
                    postupdate.Title =post.Title;
                    postupdate.Description = post.Description;
                    postupdate.Content = post.Content;
                    postupdate.Published = post.Published;
                    postupdate.Slug = post.Slug;
                    postupdate.DateUpdated = DateTime.Now;
                    if (post.CategoryIDs == null) post.CategoryIDs = new int[] { };
                    var oldcateid = postupdate.PostCategories.Select(c => c.CategoryID).ToArray();
                    var newcateid = post.CategoryIDs;
                    var removeCatePost = from postcate in postupdate.PostCategories
                                         where (!newcateid.Contains(postcate.CategoryID))
                                         select postcate;
                    _context.PostCategories.RemoveRange(removeCatePost);
                    var addCateIds = from Cateid in newcateid
                                     where !oldcateid.Contains(Cateid)
                                     select Cateid;
                    foreach(var Cateid in addCateIds)
                    {
                        _context.PostCategories.Add(new Postcategory()
                        {
                            PostID=id,
                            CategoryID=Cateid

                        });
                    }
              




                    _context.Update(postupdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                StatusMessage = "Vừa Câp Nhật Bài Viết";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", post.AuthorId);
            return View(post);
        }

        // GET: Blog/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Blog/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            StatusMessage = "Bạn vừa xóa bài viết : " + post.Title;
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
