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
    public class InputRepository : Repository<Input>, IInputRepository
    {
        private ApplicationDbContext _db;
        public InputRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Input obj)
        {
            _db.Inputs.Update(obj);
        }
    }
}
