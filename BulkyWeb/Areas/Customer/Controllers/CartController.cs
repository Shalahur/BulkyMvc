﻿using System.Security.Claims;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private ShoppingCartVM ShoppingCartVM { get; set; }

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
            OrderHeader = new OrderHeader()
        };

        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        return View(ShoppingCartVM);
    }

    public IActionResult Plus(int cartId)
    {
        ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        shoppingCart.Count += 1;
        _unitOfWork.ShoppingCart.Update(shoppingCart);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

        if (shoppingCart.Count <= 1)
        {
            _unitOfWork.ShoppingCart.Remove(shoppingCart);
        }
        else
        {
            shoppingCart.Count -= 1;
            _unitOfWork.ShoppingCart.Update(shoppingCart);
        }

        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

        _unitOfWork.ShoppingCart.Remove(shoppingCart);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
            OrderHeader = new OrderHeader()
        };

        ApplicationUser applicationUserFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

        if (applicationUserFromDb != null)
        {
            ShoppingCartVM.OrderHeader.ApplicationUser = applicationUserFromDb;
            ShoppingCartVM.OrderHeader.Name = applicationUserFromDb.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = applicationUserFromDb.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = applicationUserFromDb.StreetAddress;
            ShoppingCartVM.OrderHeader.City = applicationUserFromDb.City;
            ShoppingCartVM.OrderHeader.State = applicationUserFromDb.State;
            ShoppingCartVM.OrderHeader.PostalCode = applicationUserFromDb.PostalCode;
        }
        else
        {
            ShoppingCartVM.OrderHeader.ApplicationUser = new ApplicationUser();
        }


        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);

            ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        }

        return View(ShoppingCartVM);
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }
        else if (shoppingCart.Count <= 100)
        {
            return shoppingCart.Product.Price50;
        }
        else
        {
            return shoppingCart.Product.Price100;
        }
    }
}