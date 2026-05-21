using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using TestApp.Data;

namespace TestApp.Controllers
{
    public class CartController : Controller
    {
        //private readonly AppDbContext _context;
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context) => _context = context;

        public IActionResult Index()
        {
            var userId =User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartitem= _context.CartItems.Where(x => x.userid == userId).ToList();
            //User?.Identity?.Name;
            var items = _context.CartItems
                                .Include(ci => ci.Product)
                                .Where(ci => ci.userid == userId)
                                .ToList();
            return View(items);
        }

        public async Task<IActionResult> plus(int Id)
        {
            var cdb = _context.CartItems.FirstOrDefault(u => u.Id == Id);
            if(cdb == null)            
               return NotFound();

                cdb.Quantity +=1;
            _context.CartItems.Update(cdb);
                await _context.SaveChangesAsync();
           
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> minus(int Id)
        {
            var cdb = _context.CartItems.FirstOrDefault(u => u.Id == Id);
            if (cdb == null)
                return NotFound();
            var userid=User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (cdb.Quantity <= 1)
            {
                _context.CartItems.Remove(cdb);
                await _context.SaveChangesAsync();
                var count = _context.CartItems.Where(x => x.userid == userid).Count();
                HttpContext.Session.SetInt32(SD.SessionCart, count);
            }
            else
            {
                cdb.Quantity -= 1;
                _context.CartItems.Update(cdb);
                await _context.SaveChangesAsync();

               
            }
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Remove(int Id)
        {
            var cdb = _context.CartItems.FirstOrDefault(u => u.Id == Id);
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.CartItems.Remove(cdb);
            HttpContext.Session.SetInt32(SD.SessionCart, _context.CartItems.Count(x=>x.userid==userid)-1);
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }





    }
}
