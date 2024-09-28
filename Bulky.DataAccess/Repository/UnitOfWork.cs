using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;

namespace Bulky.DataAccess.Repository;

public class UnitOfWork: IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    public ICategoryRepository category { get; private set; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        category = new CategoryRepository(_db);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}