using StockCheckSheet.DataAccess.Data;
using StockCheckSheet.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCheckSheet.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public IInputRepository Input {  get; private set; }
        public IOutputRepository Output { get; private set; }
        public IStockRepository Stock { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Input = new InputRepository(_db);
            Output = new OutputRepository(_db);
            Stock = new StockRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
