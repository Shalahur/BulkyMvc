using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _db;

    public CategoryController(IUnitOfWork db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        List<Category> categories = _db.Category.GetAll().ToList();
        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        // for custom error message
        if (category.Name == category.DisplayOrder.ToString())
        {
            ModelState.AddModelError("Name", "Name and Display Order must be different.");
        }

        if (ModelState.IsValid)
        {
            _db.Category.Add(category);
            _db.Save();
            TempData["Success"] = "Category created successfully.";
            return RedirectToAction("Index");
        }

        return View(category);
    }

    public IActionResult Edit(int id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        Category? category = _db.Category.Get(u => u.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _db.Category.Update(category);
            _db.Save();
            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction("Index");
        }

        return View(category);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0) return NotFound();

        Category? category = _db.Category.Get(u => u.Id == id);

        if (category == null) return NotFound();

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteCategory(int? id)
    {
        if (id == null || id == 0) return NotFound();

        Category? category = _db.Category.Get(u => u.Id == id);

        if (category == null) return NotFound();

        _db.Category.Remove(category);
        _db.Save();
        TempData["Success"] = "Category deleted successfully.";
        return RedirectToAction("Index");
    }
}