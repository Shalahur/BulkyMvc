﻿using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db;

    public CategoryController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        List<Category> categories = _db.Categories.ToList();
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
            _db.Categories.Add(category);
            _db.SaveChanges();
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

        Category? category = _db.Categories.Find(id);

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
            _db.Categories.Update(category);
            _db.SaveChanges();
            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction("Index");
        }

        return View(category);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0) return NotFound();

        Category? category = _db.Categories.Find(id);

        if (category == null) return NotFound();

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteCategory(int? id)
    {
        if (id == null || id == 0) return NotFound();

        Category? category = _db.Categories.Find(id);

        if (category == null) return NotFound();

        _db.Categories.Remove(category);
        _db.SaveChanges();
        TempData["Success"] = "Category deleted successfully.";
        return RedirectToAction("Index");
    }
}