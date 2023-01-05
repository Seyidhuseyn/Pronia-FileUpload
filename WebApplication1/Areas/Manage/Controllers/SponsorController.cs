using Microsoft.AspNetCore.Mvc;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SponsorController : Controller
    {
        readonly AppDbContext _context;
        public SponsorController(AppDbContext context)
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
        public IActionResult Create(CreateSponsorVM sponsorVM)
        {
            if (!ModelState.IsValid) return View();
            IFormFile file = sponsorVM.Image;
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
            Sponsor sponsor = new Sponsor { Order = sponsorVM.Order, Name = sponsorVM.Name, ImageUrl = fileName };
            if (_context.Sponsors.Any(s => s.Order == sponsor.Order))
            {
                ModelState.AddModelError("Order", $"{sponsor.Order} sirasinda artiq sponsor movcuddur.");
                return View();
            }
            _context.Sponsors.Add(sponsor);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0) return BadRequest();
            Sponsor sponsor = _context.Sponsors.Find(id);
            if (sponsor is null) return NotFound();
            return View(sponsor);
        }
        [HttpPost]
        public IActionResult Update(int id, Sponsor sponsor)
        {
            if (id == null || id == 0 || id != sponsor.Id || sponsor is null) return BadRequest();
            if (!ModelState.IsValid) return View();
            Sponsor anotherSponsor = _context.Sponsors.FirstOrDefault(s => s.Order == sponsor.Order);
            if (anotherSponsor != null)
            {
                anotherSponsor.Order = _context.Sponsors.Find(id).Order;
            }
            Sponsor exist = _context.Sponsors.Find(sponsor.Id);
            exist.Order = sponsor.Order;
            exist.Name = sponsor.Name;
            exist.ImageUrl = sponsor.ImageUrl;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id is null) return BadRequest();

            Sponsor sponsor = _context.Sponsors.Find(id);
            if (sponsor is null) return NotFound();
            _context.Sponsors.Remove(sponsor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
