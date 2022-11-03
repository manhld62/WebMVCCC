using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaZorWebxxx.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaZorWebxxx.Controllers
{
    public class PlanetController : Controller
    {
        private readonly Plainetservice _planetservice;
        private readonly ILogger<PlanetController> _logger;

        public PlanetController(Plainetservice planetservice, ILogger<PlanetController> logger)
        {
            _planetservice = planetservice;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [BindProperty(SupportsGet =true,Name ="action")]
        public string Name { set; get; }
        public IActionResult Mercury()
        {
            var plainet = _planetservice.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail",plainet);
        }
        public IActionResult Earth()
        {
            var plainet = _planetservice.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail", plainet);
        }
        public IActionResult Venus()
        {
            var plainet = _planetservice.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail", plainet);
        }
        public IActionResult PlanetInfo(int id)
        {
            var planet = _planetservice.Where(p => p.Id == id).FirstOrDefault();
            return View("Detail", planet);
        }
    }
}
