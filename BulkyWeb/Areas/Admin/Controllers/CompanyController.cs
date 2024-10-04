using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    public readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        List<Company> companies = _unitOfWork.Company.GetAll().ToList();
        return View(companies);
    }

    public IActionResult Upsert(int? Id)
    {
        Company company = new Company();

        if (Id != null && Id != 0)
        {
            company = _unitOfWork.Company.Get(u => u.Id == Id);
        }

        return View(company);
    }

    [HttpPost]
    public IActionResult Upsert(Company company)
    {
        if (ModelState.IsValid)
        {
            if (company.Id == 0)
            {
                _unitOfWork.Company.Add(company);
                TempData["Success"] = "Company created successfully.";
            }
            else
            {
                _unitOfWork.Company.Update(company);
                TempData["Success"] = "Company update successfully.";
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        return View(company);
    }

    #region API Calls

    [HttpGet]
    public IActionResult GetAll()
    {
        List<Company> companies = _unitOfWork.Company.GetAll().ToList();
        return Json(new
        {
            data = companies
        });
    }

    [HttpDelete]
    public IActionResult Delete(int? Id)
    {
        var companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == Id);
        if (companyToBeDeleted == null)
        {
            return Json(new { success = false, message = "Error while deleting." });
        }

        _unitOfWork.Company.Remove(companyToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Company deleted successfully." });
    }

    #endregion
}