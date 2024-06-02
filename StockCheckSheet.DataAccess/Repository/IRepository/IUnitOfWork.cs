using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCheckSheet.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IInputRepository Input {  get; }
        IOutputRepository Output { get; }
        IStockRepository Stock { get; }

        void Save();
    }
}
