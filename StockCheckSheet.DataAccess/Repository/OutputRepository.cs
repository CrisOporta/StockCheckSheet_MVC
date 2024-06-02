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
    public class OutputRepository : Repository<Output>, IOutputRepository
    {
        private ApplicationDbContext _db;
        public OutputRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Output obj)
        {
            _db.Outputs.Update(obj);
        }
    }
}
