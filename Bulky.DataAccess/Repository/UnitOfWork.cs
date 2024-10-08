﻿using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;

namespace Bulky.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public ICompanyRepository Company { get; }
    public IShoppingCartRepository ShoppingCart { get; }
    public IApplicationUserRepository ApplicationUser { get; }
    public IOrderHeaderRepository OrderHeader { get; }
    public IOrderDetailRepository OrderDetail { get; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Company = new CompanyRepository(_db);
        Category = new CategoryRepository(_db);
        Product = new ProductRepository(_db);
        ShoppingCart = new ShoppingCartRepository(_db);
        ApplicationUser = new ApplicationUserRepository(_db);
        OrderHeader = new OrderHeaderRepository(_db);
        OrderDetail = new OrderDetailsRepository(_db);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}