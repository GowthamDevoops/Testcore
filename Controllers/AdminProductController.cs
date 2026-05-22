using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TestApp.Data;
using TestApp.Models;

namespace TestApp.Controllers
{
   // [Authorize(Roles ="Admin")]
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
         List<Product> products =   _context.Products.ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            var pr = _context.Products.Find(product.Id);

            if (pr == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                pr.Name = product.Name;
                pr.Description = product.Description;
                pr.Price = product.Price;

                // Image Update
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    string wwwRootPath = _environment.WebRootPath;

                    // Delete old image
                    if (!string.IsNullOrEmpty(pr.Imagepath))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, pr.Imagepath.TrimStart('/'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save new image
                    string originalFileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName)
                                                .Replace(" ", "_");

                    string extension = Path.GetExtension(product.ImageFile.FileName);

                    string uniqueFileName = originalFileName + "_" + Guid.NewGuid().ToString() + extension;

                    string imageFolder = Path.Combine(wwwRootPath, "images");

                    if (!Directory.Exists(imageFolder))
                    {
                        Directory.CreateDirectory(imageFolder);
                    }

                    string filePath = Path.Combine(imageFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        product.ImageFile.CopyTo(stream);
                    }

                    pr.Imagepath = "/images/" + uniqueFileName;
                }

                _context.Products.Update(pr);
                _context.SaveChanges();
                TempData["sucess"] = "Record updated Sucessfully";
                return RedirectToAction("Index");
            }

            return View(product);
        }
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            // return View(product);
            TempData["sucess"] = "Record Deleted Sucessfully";
            return RedirectToAction("Index");
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
                    TempData["sucess"] = "Record Inserted Sucessfully";
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
