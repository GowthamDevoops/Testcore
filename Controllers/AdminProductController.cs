using Microsoft.AspNetCore.Mvc;
using TestApp.Data;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public AdminProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (product.Name.Trim().ToLower() == "test")
            {
                ModelState.AddModelError("Name", "The product name 'Test' is not allowed.");
            }
            else
            {
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    string wwwRootpath = _environment.WebRootPath;
                    string originalfilename = Path.GetFileNameWithoutExtension(product.ImageFile.FileName).Replace(" ", "_");
                    string extension = Path.GetExtension(product.ImageFile.FileName);
                    string uniqueFileName = originalfilename + "_" + Guid.NewGuid().ToString() + extension;
                    string imagefolder = Path.Combine(wwwRootpath, "images");
                    if (!Directory.Exists(imagefolder))
                    {
                        Directory.CreateDirectory(imagefolder);
                    }
                    string filePath = Path.Combine(imagefolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        product.ImageFile.CopyTo(stream);
                    }
                    product.Imagepath = "/images/" + uniqueFileName;
                    string confirmpath = Path.Combine(wwwRootpath, product.Imagepath.TrimStart('/'));
                    if (!System.IO.File.Exists(confirmpath))
                    {
                        throw new FileNotFoundException("Image file was not saved correctly.", confirmpath);
                    }

                }
                if (ModelState.IsValid)
                {
                    product.CreatedAt = DateTime.Now;

                    _context.Products.Add(product);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(product);
                }

            }

            return View(product);


        }
    }
}
