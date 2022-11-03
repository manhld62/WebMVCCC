using Microsoft.AspNetCore.Mvc;
using RaZorWebxxx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaZorWebxxx.Views.Shared.Components.Categorysidebar
{
    [ViewComponent]
    public class Categorysidebar : ViewComponent
    {
        public class CategorysidebarData { 
            public List<Category> Categories { set; get; }
            public int level { set; get; }
            public string categoryslug { set; get; }
        }

        public IViewComponentResult Invoke(CategorysidebarData data)
        {
            return View(data);
        }
    
    }

}
