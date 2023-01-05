using Microsoft.AspNetCore.Mvc;
using System;
using WebApplication1.DAL;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext _context { get; }

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //return View(_context.Categories.ToList());
            return View();
        }
    }
}
