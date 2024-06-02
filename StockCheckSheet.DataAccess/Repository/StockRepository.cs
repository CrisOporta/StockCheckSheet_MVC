using StockCheckSheet.DataAccess.Data;
using StockCheckSheet.DataAccess.Repository.IRepository;
using StockCheckSheetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCheckSheet.DataAccess.Repository
{
    public class StockRepository : Repository<Stock>, IStockRepository
    {
        private ApplicationDbContext _db;
        public StockRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Stock obj)
        {
            _db.Stocks.Update(obj);
        }
    }
}
