﻿using Microsoft.AspNetCore.Mvc;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class BannerController : Controller
    {
        readonly AppDbContext _context;
        public BannerController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Sliders.OrderByDescending(s => s.Order));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateBannerVM bannerVM)
        {
            if (!ModelState.IsValid) return View();
            IFormFile file = bannerVM.Image;
            if (!file.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Image", "Yuklediyiniz fal shekil deyil");
                return View();
            }
            if (file.Length > 200 * 1024)
            {
                ModelState.AddModelError("Image", "Shekilin olcusu 200 kb-dan artiq ola bilmez.");
                return View();
            }
            //string fileName = Guid.NewGuid().ToString() + (file.FileName.Length>64 ? file.FileName.Substring(0,64) : file.FileName);
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            using (var stream = new FileStream("C:\\Users\\Asus\\Desktop\\code academy\\Pronia\\WebApplication1\\WebApplication1\\wwwroot\\" + fileName, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            Banner banner = new Banner { Order = bannerVM.Order, PrimaryTitle = bannerVM.PrimaryTitle, SecondaryTitle = bannerVM.SecondaryTitle, ImageUrl = fileName };
            if (_context.Banners.Any(s => s.Order == banner.Order))
            {
                ModelState.AddModelError("Order", $"{banner.Order} sirasinda artiq banner movcuddur.");
                return View();
            }
            _context.Banners.Add(banner);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0) return BadRequest();
            Banner banner = _context.Banners.Find(id);
            if (banner is null) return NotFound();
            return View(banner);
        }
        [HttpPost]
        public IActionResult Update(int id, Banner banner)
        {
            if (id == null || id == 0 || id != banner.Id || banner is null) return BadRequest();
            if (!ModelState.IsValid) return View();
            Banner anotherBanner = _context.Banners.FirstOrDefault(s => s.Order == banner.Order);
            if (anotherBanner != null)
            {
                anotherBanner.Order = _context.Banners.Find(id).Order;
            }
            Banner exist = _context.Banners.Find(banner.Id);
            exist.Order = banner.Order;
            exist.PrimaryTitle = banner.PrimaryTitle;
            exist.SecondaryTitle = banner.SecondaryTitle;
            exist.ImageUrl = banner.ImageUrl;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id is null) return BadRequest();

            Banner banner = _context.Banners.Find(id);
            if (banner is null) return NotFound();
            _context.Banners.Remove(banner);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
