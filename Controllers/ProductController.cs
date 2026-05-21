using Microsoft.AspNetCore.Mvc;
using TestApp.Models;
using TestApp.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TestApp.Controllers
{
   
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userid != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _context.CartItems.Where(c=>c.userid == userid).Count());
            }
            var products = _context.Products.ToList();
            return View(products);
        }
        [Authorize]
        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(CartItem cartItem)
        {
                var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ct = _context.CartItems.FirstOrDefault(x => x.ProductId == cartItem.ProductId && x.userid== userid);
            if (ct != null)
            {
                ct.Quantity += cartItem.Quantity;
                _context.CartItems.Update(ct);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                cartItem.Id = 0;
                cartItem.userid = userid;
                _context.CartItems.Add(cartItem);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Product");
            }
              
           
           
        }


    }
}

