using StockCheckSheetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCheckSheet.DataAccess.Repository.IRepository
{
    public interface IInputRepository : IRepository<Input>
    {
        void Update(Input obj);
    }
}
