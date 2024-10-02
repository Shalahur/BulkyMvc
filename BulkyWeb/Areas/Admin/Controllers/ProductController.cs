using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        List<Product> products = _unitOfWork.Product.GetAll().ToList();
        return View(products);
    }

    public IActionResult Upsert(int? Id)
    {
        ProductVM productVM = new()
        {
            Categories = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Product = new Product()
        };

        if (Id != null && Id != 0)
        {
            productVM.Product = _unitOfWork.Product.Get(u => u.Id == Id);
        }

        return View(productVM);
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM productVM, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            if (imageFile != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string path = Path.Combine(wwwRootPath, @"images\product");

                using (var filestream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    imageFile.CopyTo(filestream);
                }

                productVM.Product.ImageUrl = @"\images\product\" + fileName;
            }
            
            _unitOfWork.Product.Add(productVM.Product);
            _unitOfWork.Save();
            TempData["Success"] = "Product created successfully.";
            return RedirectToAction("Index");
        }
        else
        {
            productVM.Categories = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(productVM);
        }
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0) return NotFound();

        Product? product = _unitOfWork.Product.Get(u => u.Id == id);

        if (product == null) return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteCategory(int? id)
    {
        if (id == null || id == 0) return NotFound();

        Product? product = _unitOfWork.Product.Get(u => u.Id == id);

        if (product == null) return NotFound();

        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();
        TempData["Success"] = "Product deleted successfully.";
        return RedirectToAction("Index");
    }
}